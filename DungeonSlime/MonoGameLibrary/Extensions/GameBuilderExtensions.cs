using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Concurrency;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Pooling;
using MonoGameLibrary.Extensions.MonoGame.Audio;
using MonoGameLibrary.Extensions.MonoGame.Input;
using MonoGameLibrary.Extensions.General.Scenes;
using MonoGameLibrary.Extensions.General.States;

namespace MonoGameLibrary.Extensions {
    /// <summary>
    /// Convenience extensions for registering optional services and modules. 
    /// </summary>
    public static class GameBuilderExtensions {
        /// <summary>
        /// Registers a default set of services (logger, profiler, thread pool, cancellation, loading progress, object pool) if they are not already registered. 
        /// </summary>
        /// <param name="builder">The game builder instance.</param>
        /// <param name="logger">Optional logger implementation. If null, <see cref="ConsoleLogger"/> is used. </param>
        /// <param name="profiler">Optional profiler implementation. If null, <see cref="NoOperationProfiler"/> is used. </param>
        /// <param name="poolThread">Optional thread pool implementation. If null, <see cref="DefaultThreadPool"/> is used. </param>
        /// <param name="serviceCancellation">Optional cancellation service. If null, <see cref="DefaultCancellationService"/> is used. </param>
        /// <param name="progressLoading">Optional loading progress service. If null, <see cref="DefaultLoadingProgress"/> is used. </param>
        /// <param name="factoryObjectPool">Optional object pool factory. If null, <see cref="DefaultObjectPoolFactory"/> is used. </param>
        /// <returns>The game builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
        public static GameBuilder UseDefaultServices(
            this GameBuilder builder,
            Optional<ILogger> logger = default,
            Optional<IProfiler> profiler = default,
            Optional<IThreadPool> poolThread = default,
            Optional<ICancellationService> serviceCancellation = default,
            Optional<ILoadingProgress> progressLoading = default,
            Optional<IObjectPoolFactory> factoryObjectPool = default
        ) {
            ILogger loggerResolved = logger.HasValue ? logger.Value : NullLogger.Instance;
            IProfiler profilerResolved = profiler.HasValue ? profiler.Value : new NoOperationProfiler();
            IThreadPool poolResolvedThread = poolThread.HasValue ? poolThread.Value : new DefaultThreadPool();
            ICancellationService serviceResolvedCancellation = serviceCancellation.HasValue ? serviceCancellation.Value : new DefaultCancellationService();
            ILoadingProgress progressResolvedLoading = progressLoading.HasValue ? progressLoading.Value : new DefaultLoadingProgress();
            IObjectPoolFactory factoryResolvedObjectPool = factoryObjectPool.HasValue ? factoryObjectPool.Value : new DefaultObjectPoolFactory();
            
            builder.RegisterService<ILogger>(loggerResolved, flagOverwrite: false);
            builder.RegisterService<IProfiler>(profilerResolved, flagOverwrite: false);
            builder.RegisterService<IThreadPool>(poolResolvedThread, flagOverwrite: false);
            builder.RegisterService<ICancellationService>(serviceResolvedCancellation, flagOverwrite: false);
            builder.RegisterService<ILoadingProgress>(progressResolvedLoading, flagOverwrite: false);
            builder.RegisterService<IObjectPoolFactory>(factoryResolvedObjectPool, flagOverwrite: false);
            return builder;
        }
        
        /// <summary>
        /// Registers the scene manager module. 
        /// </summary>
        /// <param name="builder">The game builder instance. </param>
        /// <param name="serviceContent">The content service used by scenes. If null, the builder will try to resolve one from its registered services. </param>
        /// <param name="logger">Optional logger for the module. If null, <see cref="NullLogger"/> is used. </param>
        /// <param name="profiler">An optional profiler for performance measurements. </param>
        /// <param name="factoryContent">Optional content service factory for automatic scene content creation. 
        /// If provided, the scene service will support factory-based scene switching. </param>
        /// <returns>The game builder instance for chaining. </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null. </exception>
        /// <exception cref="InvalidOperationException">Thrown if no <see cref="IContentService"/> can be resolved. </exception>
        public static GameBuilder UseScenes(
                this GameBuilder builder, 
                IContentService serviceContent = null, 
                Optional<ILogger> logger = default, 
                Optional<IProfiler> profiler = default, 
                Optional<IContentServiceFactory> factoryContent = default
            ) {
            if (builder == null) {
                throw new System.ArgumentNullException(nameof(builder));
            }
            
            IContentService serviceResolvedContent;
            if (serviceContent != null) {
                serviceResolvedContent = serviceContent;
                builder.RegisterService<IContentService>(serviceContent);
            } else if (!builder.TryGetService<IContentService>(out serviceResolvedContent)) {
                throw new System.InvalidOperationException("No IContentService is registered. Register one before calling UseScenes.");
            }
            
            ILogger loggerToUse;
            if (logger.HasValue) {
                loggerToUse = logger.Value;
            } else {
                loggerToUse = NullLogger.Instance;
            }
            
            var serviceScene = new SceneService(
                serviceResolvedContent, new Optional<ILogger>(loggerToUse), profiler
            );
            builder.RegisterService<ISceneService>(serviceScene);
            builder.AddModule(new SceneModule(serviceScene));
            return builder;
        }
        
        /// <summary>
        /// Registers the state service and its host module.
        /// </summary>
        /// <param name="builder">The game builder.</param>
        /// <param name="order">Execution order for the module (default 0).</param>
        /// <returns>The builder for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if builder is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if IInputService is not registered.</exception>
        public static GameBuilder UseStates(this GameBuilder builder, int order = 0) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            
            if (!builder.TryGetService<IInputService>(out var serviceInput)) {
                throw new InvalidOperationException(
                    "IInputService must be registered before calling UseStates. " +
                    "Use builder.UseInput() or register manually."
                );
            }
            
            var service = new StateService();
            builder.RegisterService<IStateService>(service);
            
            var module = new StateModule(service, serviceInput, order);
            builder.AddModule(module);
            
            return builder;
        }
    }
}

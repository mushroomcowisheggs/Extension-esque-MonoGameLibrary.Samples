using System;
using System.Collections.Generic;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Concurrency;
using MonoGameLibrary.Core.Pooling;

namespace MonoGameLibrary.Core.Hosting {
    /// <summary>
    /// Provides a fluent API for configuring and building an <see cref="IGameHost"/>.
    /// Each instance can only build one host; subsequent calls to <see cref="Build"/> will throw.
    /// </summary>
    public class GameBuilder {
        private readonly object _lockBuilder = new object();
        private readonly ServiceRegistry _registryService = new ServiceRegistry();
        private readonly HashSet<object> _setModule = new HashSet<object>();
        private readonly List<object> _listModules = new List<object>();
        private Action<GameHost> _actionConfigHost = delegate { };
        private bool _flagIsBuilding = false;
        private bool _flagIsBuilt = false;
        
        /// <summary>
        /// Registers a service instance, optionally overwriting an existing registration. 
        /// </summary>
        /// <typeparam name="T">The type of the service. </typeparam>
        /// <param name="instance">The service instance. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces an existing registration. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="flagOverwrite"/> is <c>false</c> 
        /// and a service of type <typeparamref name="T"/> is already registered. </exception>
        public GameBuilder RegisterService<T>(T instance, bool flagOverwrite = false) where T : class {
            if (instance == null) { throw new ArgumentNullException(nameof(instance)); }
            lock (_lockBuilder) {
                if (_flagIsBuilt) { throw new InvalidOperationException("Build already called."); }
                if (_flagIsBuilding) { throw new InvalidOperationException("Build in progress."); }

                if (flagOverwrite) {
                    _registryService.TryRegister(instance, flagOverwrite: true);
                } else {
                    if (!_registryService.TryRegister(instance, flagOverwrite: false)) {
                        throw new InvalidOperationException($"Service of type {typeof(T).FullName} is already registered.");
                    }
                }
            }
            return this;
        }
        
        /// <summary>
        /// Adds a module to the host. Duplicate modules are rejected immediately. 
        /// </summary>
        /// <param name="module">The module instance to add. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="module"/> is null. </exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="module"/> has already been added. </exception>
        public GameBuilder AddModule(object module) {
            if (module == null) { throw new ArgumentNullException(nameof(module)); }
            lock (_lockBuilder) {
                if (_flagIsBuilt) { throw new InvalidOperationException("Build already called."); }
                if (_flagIsBuilding) { throw new InvalidOperationException("Build in progress."); }

                if (_setModule.Contains(module)) {
                    throw new InvalidOperationException($"Module {module.GetType().FullName} already added.");
                }
                _setModule.Add(module);
                _listModules.Add(module);
            }
            return this;
        }
        
        /// <summary>
        /// Provides a callback to configure the <see cref="GameHost"/> before modules are added. 
        /// </summary>
        /// <param name="configAction">The action to run against the host. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder ConfigureHost(Action<GameHost> configAction) {
            lock (_lockBuilder) {
                if (_flagIsBuilt) { throw new InvalidOperationException("Build already called."); }
                if (_flagIsBuilding) { throw new InvalidOperationException("Build in progress."); }
                if (configAction == null) { throw new ArgumentNullException(nameof(configAction)); }
                _actionConfigHost = configAction;
            }
            return this;
        }
        
        /// <summary>
        /// Registers a logger service. 
        /// </summary>
        /// <param name="logger">The logger implementation. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces any previously registered <see cref="ILogger"/>. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder UseLogger(ILogger logger, bool flagOverwrite = false) { RegisterService(logger, flagOverwrite); return this; }
        
        /// <summary>
        /// Registers a profiler service. 
        /// </summary>
        /// <param name="profiler">The profiler implementation. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces any previously registered <see cref="IProfiler"/>. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder UseProfiler(IProfiler profiler, bool flagOverwrite = false) { RegisterService(profiler, flagOverwrite); return this; }
        
        /// <summary>
        /// Registers a thread pool service. 
        /// </summary>
        /// <param name="poolThread">The thread pool implementation. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces any previously registered <see cref="IThreadPool"/>. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder UseThreadPool(IThreadPool poolThread, bool flagOverwrite = false) { RegisterService(poolThread, flagOverwrite); return this; }
        
        /// <summary>
        /// Registers a cancellation service. 
        /// </summary>
        /// <param name="serviceCancellation">The cancellation service implementation. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces any previously registered <see cref="ICancellationService"/>. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder UseCancellationService(ICancellationService serviceCancellation, bool flagOverwrite = false) { RegisterService(serviceCancellation, flagOverwrite); return this; }
        
        /// <summary>
        /// Registers a loading progress service. 
        /// </summary>
        /// <param name="progressLoading">The loading progress implementation. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces any previously registered <see cref="ILoadingProgress"/>. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder UseLoadingProgress(ILoadingProgress progressLoading, bool flagOverwrite = false) { RegisterService(progressLoading, flagOverwrite); return this; }
        
        /// <summary>
        /// Registers an object pool factory. 
        /// </summary>
        /// <param name="factory">The pool factory implementation. </param>
        /// <param name="flagOverwrite">If <c>true</c>, replaces any previously registered <see cref="IObjectPoolFactory"/>. </param>
        /// <returns>The current <see cref="GameBuilder"/> for chaining. </returns>
        public GameBuilder UseObjectPoolFactory(IObjectPoolFactory factoryPool, bool flagOverwrite = false) { RegisterService(factoryPool, flagOverwrite); return this; }
        
        /// <summary>
        /// Retrieves a registered service instance of the specified type. 
        /// </summary>
        /// <typeparam name="T">The service type to retrieve. </typeparam>
        /// <returns>The registered service instance. </returns>
        /// <exception cref="InvalidOperationException">Thrown if the service is not registered. </exception>
        public T GetService<T>() where T : class {
            return _registryService.Get<T>();
        }
        
        /// <summary>
        /// Tries to retrieve a registered service instance of the specified type. 
        /// </summary>
        /// <typeparam name="T">The service type to retrieve. </typeparam>
        /// <param name="instance">The registered service instance if present; otherwise null.</param>
        /// <returns><c>true</c> if the service is registered; otherwise <c>false</c>.</returns>
        public bool TryGetService<T>(out T instance) where T : class {
            return _registryService.TryGet<T>(out instance);
        }
        
        /// <summary>
        /// Clears all registered services from the builder. 
        /// This does not affect modules already added. 
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="Build"/> has already been called or is in progress. 
        /// </exception>
        public void ClearServices() {
            lock (_lockBuilder) {
                if (_flagIsBuilt) { throw new InvalidOperationException("Build already called."); }
                if (_flagIsBuilding) { throw new InvalidOperationException("Build in progress."); }
                _registryService.Clear();
            }
        }
        
        /// <summary>
        /// Builds the <see cref="IGameHost"/> with all configured services and modules. 
        /// This method can only be called once per builder instance. 
        /// </summary>
        /// <returns>An initialized <see cref="IGameHost"/>. </returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Build"/> has already been called. </exception>
        public IGameHost Build() {
            GameHost host;
            Action<GameHost> actionToRunConfig;
            List<object> modulesSnapshot;
            
            lock (_lockBuilder) {
                if (_flagIsBuilt) { throw new InvalidOperationException("Build already completed."); }
                if (_flagIsBuilding) { throw new InvalidOperationException("Build in progress."); }
                
                _flagIsBuilding = true;
                host = new GameHost(_registryService);
                actionToRunConfig = _actionConfigHost;
                modulesSnapshot = new List<object>(_listModules);
            }
            
            try {
                actionToRunConfig(host);
                
                // Set up exception forwarding for the default thread pool so that unhandled exceptions
                // are reported to the host's OnError callback. Custom IThreadPool implementations must
                // handle exception propagation themselves. 
                if (_registryService.TryGet<DefaultThreadPool>(out var threadPool)) {
                    threadPool.SetExceptionHandlerProvider(delegate() { return host.OnError; });
                }
                
                foreach (object module in modulesSnapshot) {
                    host.AddModule(module);
                }
                
                lock (_lockBuilder) { _flagIsBuilt = true; }
                return host;
            } catch {
                try { host.Dispose(); } catch { }
                lock (_lockBuilder) {
                    _registryService.Clear();
                    _flagIsBuilding = false;
                }
                throw;
            }
        }
    }
}
using System;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Adapters.MonoGame.Audio;
using MonoGameLibrary.Adapters.MonoGame.Input;
using MonoGameLibrary.Extensions.Audio;
using MonoGameLibrary.Extensions.Input;

namespace MonoGameLibrary.Adapters.MonoGame {
    public static class MonoGameBuilderExtensions {
        /// <summary>
        /// Registers the audio service and module.
        /// </summary>
        /// <param name="builder">The game builder instance.</param>
        /// <returns>The game builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
        public static GameBuilder UseAudio(this GameBuilder builder) {
            if (builder == null) {
                throw new System.ArgumentNullException(nameof(builder));
            }
            
            var serviceAudio = new AudioService();
            builder.RegisterService<IAudioService>(serviceAudio);
            builder.AddModule(new AudioModule(serviceAudio));
            return builder;
        }
        
        public static GameBuilder UseAudio(this GameBuilder builder, IContentService serviceContent = null) {
            if (builder == null) {
                throw new System.ArgumentNullException(nameof(builder));
            }
            
            var serviceAudio = new AudioService(serviceContent);
            builder.RegisterService<IAudioService>(serviceAudio);
            builder.AddModule(new AudioModule(serviceAudio));
            return builder;
        }
        
        /// <summary>
        /// Registers the input service and module.
        /// </summary>
        /// <param name="builder">The game builder instance.</param>
        /// <returns>The game builder instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
        public static GameBuilder UseInput(this GameBuilder builder) {
            if (builder == null) {
                throw new System.ArgumentNullException(nameof(builder));
            }
            
            var serviceInput = new InputService();
            builder.RegisterService<IInputService>(serviceInput);
            builder.AddModule(new InputModule(serviceInput));
            return builder;
        }
        
        /// <summary>
        /// Registers the default input mapping service with the builder.
        /// </summary>
        /// <param name="builder">The game builder instance.</param>
        /// <returns>The builder for chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if IInputService is not registered.</exception>
        public static GameBuilder UseInputMapping(this GameBuilder builder) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }
            
            if (!builder.TryGetService<IInputService>(out var serviceInput)) {
                throw new InvalidOperationException(
                    "IInputService must be registered before calling UseInputMapping. " +
                    "Use builder.UseInput() or register manually."
                );
            }
            
            var serviceMapping = new DefaultInputMappingService(serviceInput);
            builder.RegisterService<IInputMappingService>(serviceMapping);
            return builder;
        }
    }
}
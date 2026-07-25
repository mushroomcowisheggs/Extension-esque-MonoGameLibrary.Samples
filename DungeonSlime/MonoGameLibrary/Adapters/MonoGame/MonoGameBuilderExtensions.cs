using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Adapters.MonoGame.Audio;
using MonoGameLibrary.Adapters.MonoGame.Input;
using MonoGameLibrary.Extensions.MonoGame.Audio;
using MonoGameLibrary.Extensions.MonoGame.Input;

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
    }
}
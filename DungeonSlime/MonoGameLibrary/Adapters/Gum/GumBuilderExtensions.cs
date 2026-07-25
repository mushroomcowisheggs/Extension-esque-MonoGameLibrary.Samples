using System;
using Gum.Forms;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Adapters.MonoGame;
using MonoGameLibrary.Core.Hosting;

namespace MonoGameLibrary.Adapters.Gum {
    public static class GumBuilderExtensions {
        /// <summary>
        /// Configures Gum UI framework using the registered IContentService (must be a MonoGameContentService).
        /// </summary>
        /// <param name="builder">The game builder.</param>
        /// <param name="game">The running game instance.</param>
        /// <param name="version">Gum visual version.</param>
        /// <returns>The builder for chaining. </returns>
        /// <exception cref="ArgumentNullException">Thrown if builder or game is null. </exception>
        /// <exception cref="InvalidOperationException">Thrown if no IContentService is registered, or it is not a MonoGameContentService. </exception>
        public static GameBuilder UseGum(this GameBuilder builder, Game game, DefaultVisualsVersion version) {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (game == null) throw new ArgumentNullException(nameof(game));
            
            // Retrieve the registered IContentService
            if (!builder.TryGetService<IContentService>(out var serviceContent)) {
                throw new InvalidOperationException("No IContentService registered. Use builder.RegisterService<IContentService>() first.");
            }
            
            var serviceMonoGameContent = serviceContent as MonoGameContentService;
            if (serviceMonoGameContent == null) {
                throw new InvalidOperationException("Registered IContentService is not a MonoGameContentService. Gum requires a MonoGameContentService.");
            }
            
            // Create the Gum service with its dependencies
            var serviceGum = new GumService(
                game,
                version
            );
            
            // Register the service so other modules can inject IGumService if needed
            builder.RegisterService<IGumService>(serviceGum);
            
            // Add an internal module that will call Initialize during LoadContent phase
            builder.AddModule(new GumInitializationModule(serviceGum, serviceMonoGameContent.ContentManager));
            
            return builder;
        }
    }
}
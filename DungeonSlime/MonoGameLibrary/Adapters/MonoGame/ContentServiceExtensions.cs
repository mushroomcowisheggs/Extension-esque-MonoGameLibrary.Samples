using System;
using System.IO;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Extensions.MonoGame.Graphics;

namespace MonoGameLibrary.Adapters.MonoGame {
    /// <summary>
    /// Adapter-layer extensions for loading assets from files using MonoGame's content pipeline. 
    /// </summary>
    public static class ContentServiceExtensions {
        /// <summary>
        /// Loads a texture atlas or tilemap from an XML file. 
        /// </summary>
        /// <typeparam name="T">Expected return type; must be <see cref="TextureAtlas"/> or <see cref="Tilemap"/>. </typeparam>
        /// <param name="serviceContent">The content service (must be a <see cref="MonoGameContentService"/>). </param>
        /// <param name="pathFile">The relative file path (e.g., "Content/hero.atlas"). </param>
        /// <returns>A new instance of <typeparamref name="T"/>. </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceContent"/> is null. </exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="pathFile"/> is null or empty. </exception>
        /// <exception cref="InvalidOperationException">Thrown if the service is not a <see cref="MonoGameContentService"/>. </exception>
        /// <exception cref="NotSupportedException">Thrown if <typeparamref name="T"/> is not supported. </exception>
        public static T FromFile<T>(this IContentService serviceContent, string pathFile) where T : class {
            if (serviceContent == null) {
                throw new ArgumentNullException(nameof(serviceContent));
            }
            if (string.IsNullOrWhiteSpace(pathFile)) {
                throw new ArgumentException("File path cannot be null or empty. ", nameof(pathFile));
            }
            
            // Adapter layer is permitted to depend on MonoGame-specific types.
            MonoGameContentService serviceMonoGameContent = serviceContent as MonoGameContentService;
            if (serviceMonoGameContent == null) {
                throw new InvalidOperationException(
                    "This extension requires a MonoGameContentService. " +
                    "If you are not using MonoGame, implement your own file-loading logic. "
                );
            }
            
            string pathFull = Path.Combine(serviceMonoGameContent.ContentManager.RootDirectory, pathFile);
            using (Stream stream = TitleContainer.OpenStream(pathFull)) {
                if (typeof(T) == typeof(TextureAtlas)) {
                    return TextureAtlas.FromStream(serviceContent, stream) as T;
                }
                if (typeof(T) == typeof(Tilemap)) {
                    return Tilemap.FromStream(serviceContent, stream) as T;
                }
                throw new NotSupportedException(
                    $"Type {typeof(T).FullName} is not supported. Use TextureAtlas or Tilemap. "
                );
            }
        }
    }
}
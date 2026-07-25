using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Core.Lifecycle;

namespace MonoGameLibrary.Adapters.MonoGame {
    /// <summary>
    /// A MonoGame-specific rendering context that provides a <see cref="SpriteBatch"/>.
    /// </summary>
    public class MonoGameRenderContext : IRenderContext {
        /// <summary>
        /// Gets the MonoGame <see cref="SpriteBatch"/> used for drawing.
        /// </summary>
        public SpriteBatch SpriteBatch { get; }
        
        /// <inheritdoc />
        public object NativeContext { get { return SpriteBatch; } }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameRenderContext"/> class. 
        /// </summary>
        /// <param name="batchSprite">The <see cref="SpriteBatch"/> to use. </param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="batchSprite"/> is null. </exception>
        public MonoGameRenderContext(SpriteBatch batchSprite) {
            if (batchSprite == null) { throw new System.ArgumentNullException(nameof(batchSprite)); }
            SpriteBatch = batchSprite;
        }
    }
}
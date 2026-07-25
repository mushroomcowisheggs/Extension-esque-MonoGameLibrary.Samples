using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Extensions.MonoGame.Graphics {
    /// <summary>
    /// Represents a rectangular region inside a texture.
    /// </summary>
    public class TextureRegion {
        /// <summary>
        /// Gets or sets the backing texture.
        /// </summary>
        public Texture2D Texture { get; set; }
        
        /// <summary>
        /// Gets or sets the source rectangle.
        /// </summary>
        public Rectangle SourceRectangle { get; set; }
        
        /// <summary>
        /// Gets the width of the region.
        /// </summary>
        public int Width { get { return SourceRectangle.Width; } }
        
        /// <summary>
        /// Gets the height of the region.
        /// </summary>
        public int Height { get { return SourceRectangle.Height; } }
        
        public float TopTextureCoordinate { get { return (float)SourceRectangle.Top / Texture.Height; } }
        public float BottomTextureCoordinate { get { return (float)SourceRectangle.Bottom / Texture.Height; } }
        public float LeftTextureCoordinate { get { return (float)SourceRectangle.Left / Texture.Width; } }
        public float RightTextureCoordinate { get { return (float)SourceRectangle.Right / Texture.Width; } }
        
        /// <summary>
        /// Creates a new texture region.
        /// </summary>
        public TextureRegion() {
        }
        
        /// <summary>
        /// Creates a new texture region with the specified source rectangle.
        /// </summary>
        public TextureRegion(Texture2D texture, Rectangle rectangleSource) {
            Texture = texture;
            SourceRectangle = rectangleSource;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Extensions.MonoGame.Graphics {
    /// <summary>
    /// A simple sprite wrapper around a texture region. 
    /// </summary>
    public class Sprite {
        /// <summary>
        /// Gets or sets the region this sprite renders. 
        /// </summary>
        public TextureRegion Region { get; set; }
        
        /// <summary>
        /// Gets or sets the tint color. 
        /// </summary>
        public Color Color { get; set; } = Color.White;
        
        /// <summary>
        /// Gets or sets the scale. 
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;
        
        /// <summary>
        /// Gets or sets the origin. 
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;
        
        /// <summary>
        /// Gets the width in pixels. 
        /// </summary>
        public float Width { get {
            if (Region == null) {
                return 0f;
            } else {
                return Region.Width * Scale.X;
            }
        } }
        
        /// <summary>
        /// Gets the height in pixels. 
        /// </summary>
        public float Height { get {
            if (Region == null) {
                return 0f;
            } else {
                return Region.Height * Scale.Y;
            }
        } }
        
        /// <summary>
        /// Creates a new sprite. 
        /// </summary>
        public Sprite() {
        }
        
        /// <summary>
        /// Creates a new sprite with the provided region. 
        /// </summary>
        public Sprite(TextureRegion region) {
            Region = region;
        }
        
        /// <summary>
        /// Draws the sprite with the supplied sprite batch. 
        /// </summary>
        /// <param name="batchSprite">The SpriteBatch instance used for batching draw calls. </param>
        /// <param name="position">The xy-coordinate position to render this sprite at. </param>
        public void Draw(SpriteBatch batchSprite, Vector2 position) {
            if (batchSprite == null || Region == null || Region.Texture == null) {
                return;
            }
            
            batchSprite.Draw(Region.Texture, position, Region.SourceRectangle, Color, 0f, Origin, Scale, SpriteEffects.None, 0f);
        }
    }
}

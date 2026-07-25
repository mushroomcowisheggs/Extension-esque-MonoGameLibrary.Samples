using System;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Adapters.Gum {
    /// <summary>
    /// Provides initialization and access to the Gum UI framework. 
    /// </summary>
    public interface IGumService {
        /// <summary>
        /// Initializes the Gum service with the required MonoGame dependencies. 
        /// This method must be called once before any Gum UI elements are used. 
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Updates the Gum UI tree. 
        /// </summary>
        /// <param name="timeGame">MonoGame game time. </param>
        void Update(GameTime timeGame);
        
        /// <summary>
        /// Draws the Gum UI tree. 
        /// </summary>
        void Draw();
        
        /// <summary>
        /// Clears all children from the root UI element. 
        /// </summary>
        void ClearRoot();
        
        /// <summary>
        /// Adds a UI element to the root. 
        /// </summary>
        /// <param name="element">The element to add. </param>
        void AddToRoot(global::Gum.Wireframe.GraphicalUiElement element);
        
        /// <summary>
        /// Configures the canvas size and zoom factor.
        /// </summary>
        /// <param name="width">Canvas width. </param>
        /// <param name="height">Canvas height. </param>
        /// <param name="zoom">Zoom factor. </param>
        void SetCanvas(float width, float height, float zoom);
        
        /// <summary>
        /// Configures keyboard and gamepad input for UI navigation.
        /// </summary>
        /// <param name="flagEnableKeyboard">If true, enables keyboard input.</param>
        /// <param name="flagEnableGamepad">If true, enables gamepad input.</param>
        void ConfigureInput(bool flagEnableKeyboard = true, bool flagEnableGamepad = true);
        
        /// <summary>
        /// Adds a key that triggers forward Tab navigation (default behavior).
        /// </summary>
        /// <param name="key">The key to add.</param>
        void AddTabForwardKey(Keys key);
        
        /// <summary>
        /// Adds a key that triggers reverse Tab navigation.
        /// </summary>
        /// <param name="key">The key to add.</param>
        void AddTabReverseKey(Keys key);
    }
}
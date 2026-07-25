using System;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;

namespace MonoGameLibrary.Adapters.Gum {
    /// <summary>
    /// A service that initializes and provides access to the Gum UI framework.
    /// Encapsulates the static <see cref="GumService.Default"/> instance and
    /// exposes it through the <see cref="IGumService"/> interface.
    /// </summary>
    public sealed class GumService : IGumService, IDisposable {
        private readonly Game _game;
        private readonly DefaultVisualsVersion _version;
        private readonly object _lock = new object();
        private bool _flagInitialized;
        private bool _flagDisposed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GumService"/> class. 
        /// </summary>
        /// <param name="game">The running MonoGame game instance. </param>
        /// <param name="version">The Gum visual version. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="game"/> or <paramref name="managerContent"/> is null. </exception>
        public GumService(Game game, DefaultVisualsVersion version) {
            if (game == null) {
                throw new ArgumentNullException(nameof(game));
            }
            
            _game = game;
            _version = version;
        }

        /// <inheritdoc />
        public void Initialize() {
            lock (_lock) {
                if (_flagInitialized) {
                    return;
                }
                
                // Initialize the global GumService instance with the MonoGame host.
                global::MonoGameGum.GumService.Default.Initialize(_game, _version);
                
                _flagInitialized = true;
            }
        }
        
        /// <inheritdoc />
        public void Update(GameTime timeGame) {
            EnsureInitialized();
            global::MonoGameGum.GumService.Default.Update(timeGame);
        }
        
        /// <inheritdoc />
        public void Draw() {
            EnsureInitialized();
            global::MonoGameGum.GumService.Default.Draw();
        }
        
        /// <inheritdoc />
        public void ClearRoot() {
            EnsureInitialized();
            global::MonoGameGum.GumService.Default.Root.Children.Clear();
        }
        
        /// <inheritdoc />
        public void AddToRoot(global::Gum.Wireframe.GraphicalUiElement element) {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            EnsureInitialized();
            global::MonoGameGum.GumService.Default.Root.Children.Add(element);
        }
        
        /// <inheritdoc />
        public void SetCanvas(float width, float height, float zoom) {
            EnsureInitialized();
            global::MonoGameGum.GumService.Default.CanvasWidth = width;
            global::MonoGameGum.GumService.Default.CanvasHeight = height;
            global::MonoGameGum.GumService.Default.Renderer.Camera.Zoom = zoom;
        }
        
        /// <inheritdoc />
        public void ConfigureInput(bool flagEnableKeyboard = true, bool flagEnableGamepad = true) {
            EnsureInitialized();
            
            // Keyboard input
            if (flagEnableKeyboard) {
                FrameworkElement.KeyboardsForUiControl.Add(global::MonoGameGum.GumService.Default.Keyboard);
            }
            
            // Gamepad input
            if (flagEnableGamepad) {
                FrameworkElement.GamePadsForUiControl.AddRange(global::MonoGameGum.GumService.Default.Gamepads);
            }
        }
        
        /// <inheritdoc />
        public void AddTabForwardKey(Keys key) {
            EnsureInitialized();
            FrameworkElement.TabKeyCombos.Add(new KeyCombo { PushedKey = key });
        }
        
        /// <inheritdoc />
        public void AddTabReverseKey(Keys key) {
            EnsureInitialized();
            FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo { PushedKey = key });
        }
        
        private void EnsureInitialized() {
            if (!_flagInitialized) { throw new InvalidOperationException("GumService must be initialized before use."); }
        }
        
        /// <summary>
        /// Disposes the service (no unmanaged resources to release).
        /// </summary>
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            // If the global GumService supports IDisposable, dispose it here.
            // Otherwise, simply clear references.
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
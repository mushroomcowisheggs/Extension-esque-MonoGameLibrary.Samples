using System;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.Input;
using MonoGameLibrary.Extensions.UserInterface;

namespace MonoGameLibrary.Adapters.Gum {
    /// <summary>
    /// Gum implementation of <see cref="IUserInterfaceService"/>.
    /// </summary>
    public sealed class GumService : IUserInterfaceService, ITabNavigationSupport, IDisposable {
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
        public void Update(FrameTime timeFrame) {
            EnsureInitialized();
            GameTime timeGame = new GameTime(timeFrame.TotalTimeSpan, timeFrame.DeltaTimeSpan);
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
        public void AddToRoot(object element) {
            if (element == null) {
                throw new ArgumentNullException(nameof(element));
            }
            EnsureInitialized();
            GraphicalUiElement gue = element as GraphicalUiElement;
            if (gue == null) {
                throw new ArgumentException("Element must be a GraphicalUiElement.", nameof(element));
            }
            global::MonoGameGum.GumService.Default.Root.Children.Add(gue);
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
        public void AddTabForwardKey(KeyCode codeKey) {
            EnsureInitialized();
            Keys key = ConvertKeyCode(codeKey);
            FrameworkElement.TabKeyCombos.Add(new KeyCombo { PushedKey = key });
        }
        
        /// <inheritdoc />
        public void AddTabReverseKey(KeyCode codeKey) {
            EnsureInitialized();
            Keys key = ConvertKeyCode(codeKey);
            FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo { PushedKey = key });
        }
        
        private static Keys ConvertKeyCode(KeyCode codeKey) {
            switch (codeKey) {
                case KeyCode.None: return Keys.None;
                case KeyCode.A: return Keys.A;
                case KeyCode.B: return Keys.B;
                case KeyCode.C: return Keys.C;
                case KeyCode.D: return Keys.D;
                case KeyCode.E: return Keys.E;
                case KeyCode.F: return Keys.F;
                case KeyCode.G: return Keys.G;
                case KeyCode.H: return Keys.H;
                case KeyCode.I: return Keys.I;
                case KeyCode.J: return Keys.J;
                case KeyCode.K: return Keys.K;
                case KeyCode.L: return Keys.L;
                case KeyCode.M: return Keys.M;
                case KeyCode.N: return Keys.N;
                case KeyCode.O: return Keys.O;
                case KeyCode.P: return Keys.P;
                case KeyCode.Q: return Keys.Q;
                case KeyCode.R: return Keys.R;
                case KeyCode.S: return Keys.S;
                case KeyCode.T: return Keys.T;
                case KeyCode.U: return Keys.U;
                case KeyCode.V: return Keys.V;
                case KeyCode.W: return Keys.W;
                case KeyCode.X: return Keys.X;
                case KeyCode.Y: return Keys.Y;
                case KeyCode.Z: return Keys.Z;
                case KeyCode.Space: return Keys.Space;
                case KeyCode.Enter: return Keys.Enter;
                case KeyCode.Escape: return Keys.Escape;
                case KeyCode.Tab: return Keys.Tab;
                case KeyCode.Backspace: return Keys.Back;
                case KeyCode.Up: return Keys.Up;
                case KeyCode.Down: return Keys.Down;
                case KeyCode.Left: return Keys.Left;
                case KeyCode.Right: return Keys.Right;
                case KeyCode.F1: return Keys.F1;
                case KeyCode.F2: return Keys.F2;
                case KeyCode.F3: return Keys.F3;
                case KeyCode.F4: return Keys.F4;
                case KeyCode.F5: return Keys.F5;
                case KeyCode.F6: return Keys.F6;
                case KeyCode.F7: return Keys.F7;
                case KeyCode.F8: return Keys.F8;
                case KeyCode.F9: return Keys.F9;
                case KeyCode.F10: return Keys.F10;
                case KeyCode.F11: return Keys.F11;
                case KeyCode.F12: return Keys.F12;
                default: return Keys.None;
            }
        }
        
        private void EnsureInitialized() {
            if (!_flagInitialized) {
                throw new InvalidOperationException("GumService must be initialized before use.");
            }
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
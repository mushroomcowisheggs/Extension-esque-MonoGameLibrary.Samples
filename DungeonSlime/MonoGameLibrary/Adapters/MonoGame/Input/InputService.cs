using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.Input;

namespace MonoGameLibrary.Adapters.MonoGame.Input {
    /// <summary>
    /// MonoGame implementation of <see cref="IInputService"/>. 
    /// </summary>
    public sealed class InputService : IInputService, IDisposable {
        private readonly object _lock = new object();
        private readonly Dictionary<Microsoft.Xna.Framework.PlayerIndex, GamePadState> _dictionaryCurrentGamePadStates;
        private readonly Dictionary<Microsoft.Xna.Framework.PlayerIndex, GamePadState> _dictionaryPreviousGamePadStates;
        private int _countFrame;
        private bool _flagDisposed = false;
        private KeyboardState _stateKeyboardCurrent;
        private KeyboardState _stateKeyboardPrevious;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InputService"/> class.
        /// </summary>
        public InputService() {
            _dictionaryCurrentGamePadStates = new Dictionary<Microsoft.Xna.Framework.PlayerIndex, GamePadState>();
            _dictionaryPreviousGamePadStates = new Dictionary<Microsoft.Xna.Framework.PlayerIndex, GamePadState>();
            
            // Pre-populate with initial states for all players
            foreach (Microsoft.Xna.Framework.PlayerIndex indexPlayer in Enum.GetValues(typeof(Microsoft.Xna.Framework.PlayerIndex))) {
                GamePadState state = GamePad.GetState(indexPlayer);
                _dictionaryCurrentGamePadStates[indexPlayer] = state;
                _dictionaryPreviousGamePadStates[indexPlayer] = state;
            }
        }
        
        /// <summary>
        /// Gets the number of frames processed by the manager.
        /// </summary>
        public int FrameCount {
            get {
                lock (_lock) {
                    return _countFrame;
                }
            }
        }
        
        /// <summary>
        /// Updates the input state for the current frame. 
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame. </param>
        public void Update(FrameTime timeFrame) {
            _stateKeyboardPrevious = _stateKeyboardCurrent;
            _stateKeyboardCurrent = Keyboard.GetState();
            
            foreach (Microsoft.Xna.Framework.PlayerIndex indexPlayer in Enum.GetValues(typeof(Microsoft.Xna.Framework.PlayerIndex))) {
                _dictionaryPreviousGamePadStates[indexPlayer] = _dictionaryCurrentGamePadStates[indexPlayer];
                _dictionaryCurrentGamePadStates[indexPlayer] = GamePad.GetState(indexPlayer);
            }
            
            lock (_lock) {
                _countFrame += 1;
            }
        }
        
        /// <inheritdoc />
        public bool IsKeyDown(KeyCode codeKey) {
            Keys key = KeyCodeConverter.ToMonoGameKey(codeKey);
            return _stateKeyboardCurrent.IsKeyDown(key);
        }
        
        /// <inheritdoc />
        public bool IsKeyUp(KeyCode codeKey) {
            Keys key = KeyCodeConverter.ToMonoGameKey(codeKey);
            return _stateKeyboardCurrent.IsKeyUp(key);
        }
        
        /// <inheritdoc />
        public bool WasKeyJustPressed(KeyCode codeKey) {
            Keys key = KeyCodeConverter.ToMonoGameKey(codeKey);
            return _stateKeyboardCurrent.IsKeyDown(key) && _stateKeyboardPrevious.IsKeyUp(key);
        }
        
        /// <inheritdoc />
        public bool WasKeyJustReleased(KeyCode codeKey) {
            Keys key = KeyCodeConverter.ToMonoGameKey(codeKey);
            return _stateKeyboardCurrent.IsKeyUp(key) && _stateKeyboardPrevious.IsKeyDown(key);
        }
        
        /// <summary>
        /// Converts a platform-independent <see cref="GamePadButton"/> to a MonoGame <see cref="Buttons"/>. 
        /// </summary>
        /// <param name="button">The platform-independent button. </param>
        /// <returns>The corresponding MonoGame button. </returns>
        private static Buttons ConvertButton(GamePadButton button) {
            switch (button) {
                case GamePadButton.A: return Buttons.A;
                case GamePadButton.B: return Buttons.B;
                case GamePadButton.X: return Buttons.X;
                case GamePadButton.Y: return Buttons.Y;
                case GamePadButton.Start: return Buttons.Start;
                case GamePadButton.Back: return Buttons.Back;
                case GamePadButton.LeftStick: return Buttons.LeftStick;
                case GamePadButton.RightStick: return Buttons.RightStick;
                case GamePadButton.LeftShoulder: return Buttons.LeftShoulder;
                case GamePadButton.RightShoulder: return Buttons.RightShoulder;
                case GamePadButton.DPadUp: return Buttons.DPadUp;
                case GamePadButton.DPadDown: return Buttons.DPadDown;
                case GamePadButton.DPadLeft: return Buttons.DPadLeft;
                case GamePadButton.DPadRight: return Buttons.DPadRight;
                default: return Buttons.A;
            }
        }
        
        /// <summary>
        /// Converts a platform-independent <see cref="PlayerIndex"/> to a MonoGame <see cref="Microsoft.Xna.Framework.PlayerIndex"/>.
        /// </summary>
        /// <param name="indexPlayer">The platform-independent player index.</param>
        /// <returns>The corresponding MonoGame player index.</returns>
        private static Microsoft.Xna.Framework.PlayerIndex ConvertPlayerIndex(MonoGameLibrary.Extensions.Input.PlayerIndex indexPlayer) {
            switch (indexPlayer) {
                case MonoGameLibrary.Extensions.Input.PlayerIndex.One: return Microsoft.Xna.Framework.PlayerIndex.One;
                case MonoGameLibrary.Extensions.Input.PlayerIndex.Two: return Microsoft.Xna.Framework.PlayerIndex.Two;
                case MonoGameLibrary.Extensions.Input.PlayerIndex.Three: return Microsoft.Xna.Framework.PlayerIndex.Three;
                case MonoGameLibrary.Extensions.Input.PlayerIndex.Four: return Microsoft.Xna.Framework.PlayerIndex.Four;
                default: return Microsoft.Xna.Framework.PlayerIndex.One;
            }
        }
        
        /// <inheritdoc />
        public bool IsButtonDown(MonoGameLibrary.Extensions.Input.PlayerIndex indexPlayer, GamePadButton button) {
            Buttons buttonMono = ConvertButton(button);
            Microsoft.Xna.Framework.PlayerIndex indexMono = ConvertPlayerIndex(indexPlayer);
            GamePadState state = GetGamePadState(indexMono);
            return state.IsButtonDown(buttonMono);
        }
        
        /// <inheritdoc />
        public bool WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex indexPlayer, GamePadButton button) {
            Buttons buttonMono = ConvertButton(button);
            Microsoft.Xna.Framework.PlayerIndex indexMono = ConvertPlayerIndex(indexPlayer);
            GamePadState stateCurrent = GetGamePadState(indexMono);
            
            GamePadState statePrevious;
            if (!_dictionaryPreviousGamePadStates.TryGetValue(indexMono, out statePrevious)) {
                statePrevious = stateCurrent;
            }
            
            return stateCurrent.IsButtonDown(buttonMono) && statePrevious.IsButtonUp(buttonMono);
        }
        
        /// <inheritdoc />
        public bool WasButtonJustReleased(MonoGameLibrary.Extensions.Input.PlayerIndex indexPlayer, GamePadButton button) {
            Buttons buttonMono = ConvertButton(button);
            Microsoft.Xna.Framework.PlayerIndex indexMono = ConvertPlayerIndex(indexPlayer);
            GamePadState stateCurrent = GetGamePadState(indexMono);
            
            GamePadState statePrevious;
            if (!_dictionaryPreviousGamePadStates.TryGetValue(indexMono, out statePrevious)) {
                statePrevious = stateCurrent;
            }
            
            return stateCurrent.IsButtonUp(buttonMono) && statePrevious.IsButtonDown(buttonMono);
        }
        
        /// <summary>
        /// Retrieves the current gamepad state for the given MonoGame player index.
        /// </summary>
        /// <param name="indexPlayer">The MonoGame player index.</param>
        /// <returns>The current gamepad state.</returns>
        private GamePadState GetGamePadState(Microsoft.Xna.Framework.PlayerIndex indexPlayer) {
            if (_dictionaryCurrentGamePadStates.TryGetValue(indexPlayer, out var state)) {
                return state;
            }
            return GamePad.GetState(indexPlayer);
        }
        
        /// <summary>
        /// Disposes the service (no unmanaged resources).
        /// </summary>
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace MonoGameLibrary.Adapters.MonoGame.Input {
    /// <summary>
    /// A simple input service that can be driven by the host.
    /// Implements <see cref="IInputService"/> and <see cref="IUpdateable"/>.
    /// </summary>
    public sealed class InputService : IInputService, IDisposable {
        private readonly object _lock = new object();
        private readonly Dictionary<PlayerIndex, GamePadState> _dictionaryCurrentGamePadStates;
        private readonly Dictionary<PlayerIndex, GamePadState> _dictionaryPreviousGamePadStates;
        private int _countFrame;
        private bool _flagDisposed = false;
        private KeyboardState _stateKeyboardCurrent;
        private KeyboardState _stateKeyboardPrevious;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InputService"/> class.
        /// </summary>
        public InputService() {
            _dictionaryCurrentGamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            _dictionaryPreviousGamePadStates = new Dictionary<PlayerIndex, GamePadState>();

            // Pre-populate with initial states for all players
            foreach (PlayerIndex indexPlayer in Enum.GetValues(typeof(PlayerIndex))) {
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
        
        public KeyboardState KeyboardState { get { return _stateKeyboardCurrent; } }
        public KeyboardState PreviousKeyboardState { get { return _stateKeyboardPrevious; } }
        
        public bool WasKeyJustPressed(Keys key) {
            return _stateKeyboardCurrent.IsKeyDown(key) && _stateKeyboardPrevious.IsKeyUp(key);
        }
        
        public bool WasKeyJustReleased(Keys key) {
            return _stateKeyboardCurrent.IsKeyUp(key) && _stateKeyboardPrevious.IsKeyDown(key);
        }
        
        /// <summary>
        /// Advances the manager for the current frame.
        /// </summary>
        /// <param name="timeFrame">The frame timing information.</param>
        public void Update(FrameTime timeFrame) {
            _stateKeyboardPrevious = _stateKeyboardCurrent;
            _stateKeyboardCurrent = Keyboard.GetState();
            
            // Update gamepad states
            foreach (PlayerIndex indexPlayer in Enum.GetValues(typeof(PlayerIndex))) {
                _dictionaryPreviousGamePadStates[indexPlayer] = _dictionaryCurrentGamePadStates[indexPlayer];
                _dictionaryCurrentGamePadStates[indexPlayer] = GamePad.GetState(indexPlayer);
            }
            
            lock (_lock) {
                _countFrame += 1;
            }
        }
        
        /// <inheritdoc />
        public GamePadState GetGamePadState(PlayerIndex indexPlayer) {
            if (_dictionaryCurrentGamePadStates.TryGetValue(indexPlayer, out var state)) {
                return state;
            }
            return GamePad.GetState(indexPlayer);
        }
        
        /// <inheritdoc />
        public bool WasGamePadButtonJustPressed(PlayerIndex indexPlayer, Buttons button) {
            var current = GetGamePadState(indexPlayer);
            var previous = _dictionaryPreviousGamePadStates.TryGetValue(indexPlayer, out var prev) ? prev : current;
            return current.IsButtonDown(button) && previous.IsButtonUp(button);
        }
        
        /// <inheritdoc />
        public bool WasGamePadButtonJustReleased(PlayerIndex indexPlayer, Buttons button) {
            var current = GetGamePadState(indexPlayer);
            var previous = _dictionaryPreviousGamePadStates.TryGetValue(indexPlayer, out var prev) ? prev : current;
            return current.IsButtonUp(button) && previous.IsButtonDown(button);
        }
        
        /// <summary>
        /// Disposes the manager (no unmanaged resources).
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
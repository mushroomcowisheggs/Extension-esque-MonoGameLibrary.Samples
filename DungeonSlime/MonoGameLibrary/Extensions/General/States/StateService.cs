using System;
using System.Collections.Generic;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace MonoGameLibrary.Extensions.General.States {
    /// <summary>
    /// Default implementation of <see cref="IStateService"/>.
    /// </summary>
    public sealed class StateService : IStateService {
        private readonly List<State> _states = new List<State>();
        private bool _flagIsProcessing;
        private readonly Queue<Action> _queueOperation = new Queue<Action>();
        
        public State CurrentState { get { return _states.Count > 0 ? _states[_states.Count - 1] : null; } }
        
        private void SubscribeState(State state) {
            state.StateChangeRequested += OnStateChangeRequested;
        }
        
        private void UnsubscribeState(State state) {
            state.StateChangeRequested -= OnStateChangeRequested;
        }
        
        private void OnStateChangeRequested(object sender, StateChangeEventArgs arguments) {
            switch (arguments.ChangeType) {
                case StateChangeType.Push:
                Push(arguments.NewState);
                break;
                case StateChangeType.Pop:
                Pop();
                break;
                case StateChangeType.Change:
                Change(arguments.NewState);
                break;
            }
        }
        
        public void Push(State state) {
            Action operation = delegate () {
                if (_states.Count > 0)
                    _states[_states.Count - 1].Exit();
                SubscribeState(state);
                _states.Add(state);
                state.Enter();
            };
            QueueOrExecute(operation);
        }
        
        public void Pop() {
            Action operation = delegate () {
                if (_states.Count > 0) {
                    var top = _states[_states.Count - 1];
                    UnsubscribeState(top);
                    top.Exit();
                    _states.RemoveAt(_states.Count - 1);
                }
                if (_states.Count > 0) {
                    _states[_states.Count - 1].Enter();
                }
            };
            QueueOrExecute(operation);
        }
        
        public void Change(State state) {
            Action operation = delegate () {
                while (_states.Count > 0) {
                    var top = _states[_states.Count - 1];
                    UnsubscribeState(top);
                    top.Exit();
                    _states.RemoveAt(_states.Count - 1);
                }
                SubscribeState(state);
                _states.Add(state);
                state.Enter();
            };
            QueueOrExecute(operation);
        }
        
        private void QueueOrExecute(Action operation) {
            if (_flagIsProcessing) {
                _queueOperation.Enqueue(operation);
            }
            else {
                operation();
            }
        }
        
        public void Update(FrameTime timeFrame, IInputService serviceInput) {
            _flagIsProcessing = true;
            while (_queueOperation.Count > 0) {
                var op = _queueOperation.Dequeue();
                op?.Invoke();
            }
            
            for (int i = _states.Count - 1; i >= 0; i -= 1) {
                var state = _states[i];
                state.HandleInput(timeFrame, serviceInput);
                state.Update(timeFrame);
                if (state.IsBlocking) {
                    break;
                }
            }
            _flagIsProcessing = false;
        }
        
        public void Draw(FrameTime timeFrame, IRenderContext contextRender) {
            bool flagFoundOpaque = false;
            for (int i = 0; i < _states.Count; i += 1) {
                var state = _states[i];
                if (!flagFoundOpaque || state.IsTransparent) {
                    state.Draw(timeFrame, contextRender);
                }
                
                if (!state.IsTransparent) {
                    flagFoundOpaque = true;
                }
            }
        }
    }
}
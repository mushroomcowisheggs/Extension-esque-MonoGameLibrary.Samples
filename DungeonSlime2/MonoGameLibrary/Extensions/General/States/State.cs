using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace MonoGameLibrary.Extensions.General.States {
    /// <summary>
    /// Represents a game state that can be managed by a state service.
    /// </summary>
    public abstract class State : IDisposable {
        private bool _flagDisposed;
        
        /// <summary>
        /// Gets the content service used by this state.
        /// </summary>
        protected IContentService ContentService { get; }
        
        /// <summary>
        /// Gets the logger (optional).
        /// </summary>
        protected ILogger Logger { get; }
        
        /// <summary>
        /// Gets the profiler (optional).
        /// </summary>
        protected Optional<IProfiler> Profiler { get; }
        
        /// <summary>
        /// Gets the input service (optional).
        /// </summary>
        protected Optional<IInputService> InputService { get; }
        
        /// <summary>
        /// Occurs when a state change (push, pop, or change) is requested.
        /// </summary>
        public event EventHandler<StateChangeEventArgs> StateChangeRequested;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        protected State(
            IContentService contentService,
            Optional<ILogger> logger = default,
            Optional<IProfiler> profiler = default,
            Optional<IInputService> serviceInput = default
        ) {
            if (contentService == null) {
                throw new ArgumentNullException(nameof(contentService));
            }
            
            ContentService = contentService;
            Logger = logger.HasValue ? logger.Value : NullLogger.Instance;
            Profiler = profiler;
            InputService = serviceInput;
        }
        
        /// <summary>
        /// Called once to load content. Override to load textures, sounds, etc.
        /// </summary>
        public virtual void LoadContent() { }
        
        protected void RequestPush(State stateNew) {
            if (StateChangeRequested != null) {
                StateChangeRequested(this, new StateChangeEventArgs(StateChangeType.Push, stateNew));
            }
        }
        
        protected void RequestPop() {
            if (StateChangeRequested != null) {
                StateChangeRequested(this, new StateChangeEventArgs(StateChangeType.Pop, null));
            }
        }
        
        protected void RequestChange(State stateNew) {
            if (StateChangeRequested != null) {
                StateChangeRequested(this, new StateChangeEventArgs(StateChangeType.Change, stateNew));
            }
        }
        
        public virtual void HandleInput(FrameTime timeFrame, IInputService input) { }
        
        public virtual bool IsTransparent { get { return false; } }
        public virtual bool IsBlocking { get { return false; } }
        
        public virtual void Enter() { }
        public virtual void Exit() { }
        
        public abstract void Update(FrameTime timeFrame);
        public abstract void Draw(FrameTime timeFrame, IRenderContext contextRender);
        
        protected virtual void Dispose(bool flagDisposing) { }
        
        public virtual void Dispose() {
            if (_flagDisposed) { return; }
            _flagDisposed = true;
            if (ContentService is IDisposable disposable) {
                disposable.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }

    public class StateChangeEventArgs : EventArgs {
        public StateChangeType ChangeType { get; }
        public State NewState { get; }
        public StateChangeEventArgs(StateChangeType typeChange, State stateNew) {
            ChangeType = typeChange;
            NewState = stateNew;
        }
    }

    public enum StateChangeType {
        Push,
        Pop,
        Change
    }
}
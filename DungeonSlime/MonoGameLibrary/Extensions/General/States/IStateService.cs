using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace MonoGameLibrary.Extensions.General.States {
    /// <summary>
    /// Manages a stack of game states.
    /// </summary>
    public interface IStateService {
        /// <summary>
        /// Gets the current active state.
        /// </summary>
        State CurrentState { get; }
        
        /// <summary>
        /// Pushes a new state onto the stack.
        /// </summary>
        void Push(State state);
        
        /// <summary>
        /// Pops the current state from the stack.
        /// </summary>
        void Pop();
        
        /// <summary>
        /// Replaces the entire stack with a single state.
        /// </summary>
        void Change(State state);
        
        /// <summary>
        /// Updates all active states.
        /// </summary>
        void Update(FrameTime timeFrame, IInputService serviceInput);
        
        /// <summary>
        /// Draws all visible states.
        /// </summary>
        void Draw(FrameTime timeFrame, IRenderContext contextRender);
    }
}
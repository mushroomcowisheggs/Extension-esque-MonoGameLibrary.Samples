using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.MonoGame.Input {
    /// <summary>
    /// Provides input management services.
    /// </summary>
    public interface IInputService {
        /// <summary>
        /// Gets the number of frames processed. 
        /// </summary>
        int FrameCount { get; }
        
        /// <summary>
        /// Gets the current keyboard state. 
        /// </summary>
        KeyboardState KeyboardState { get; }
        
        /// <summary>
        /// Gets the previous keyboard state from the previous frame. 
        /// </summary>
        KeyboardState PreviousKeyboardState { get; }
        
        /// <summary>
        /// Gets the current gamepad state for the specified player.
        /// </summary>
        /// <param name="indexPlayer">The player index (0–3).</param>
        /// <returns>The current <see cref="GamePadState"/>.</returns>
        GamePadState GetGamePadState(PlayerIndex indexPlayer);
        
        /// <summary>
        /// Returns true if the specified key was pressed this frame and not the previous. 
        /// </summary>
        bool WasKeyJustPressed(Keys key);
        
        /// <summary>
        /// Returns true if the specified key was released this frame and not the previous. 
        /// </summary>
        bool WasKeyJustReleased(Keys key);
        
        /// <summary>
        /// Checks if a specific gamepad button was pressed during the current frame and was not pressed in the previous frame.
        /// </summary>
        /// <param name="indexPlayer">The player index.</param>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button is currently down and was up in the previous frame; otherwise, false.</returns>
        bool WasGamePadButtonJustPressed(PlayerIndex indexPlayer, Buttons button);
        
        /// <summary>
        /// Checks if a specific gamepad button was released during the current frame and was pressed in the previous frame.
        /// </summary>
        /// <param name="indexPlayer">The player index.</param>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the button is currently up and was down in the previous frame; otherwise, false.</returns>
        bool WasGamePadButtonJustReleased(PlayerIndex indexPlayer, Buttons button);
        
        void Update(FrameTime timeFrame);
    }
}
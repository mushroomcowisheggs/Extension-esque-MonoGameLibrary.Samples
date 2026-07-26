using Microsoft.Xna.Framework;

namespace DungeonSlime {
    /// <summary>
    /// Defines the contract for game-specific input abstraction, 
    /// mapping physical inputs to game actions. 
    /// </summary>
    public interface IGameController {
        /// <summary>
        /// Gets the directional input triggered by the player during the current frame. 
        /// Returns a normalized direction vector only when a direction key/button was
        /// just pressed; otherwise returns Vector2.Zero. 
        /// </summary>
        /// <returns>A Vector2 representing the direction, with components in range [-1, 1], 
        /// or Vector2.Zero if no direction input was just pressed. </returns>
        Vector2 GetDirection();

        /// <summary>
        /// Checks whether the pause action has been just triggered (Escape or Start button). 
        /// </summary>
        /// <returns>True if the pause action was just pressed; otherwise, false. </returns>
        bool Pause();

        /// <summary>
        /// Checks whether the action/confirm button has been just triggered (Enter or A button). 
        /// </summary>
        /// <returns>True if the action button was just pressed; otherwise, false. </returns>
        bool Action();
    }
}
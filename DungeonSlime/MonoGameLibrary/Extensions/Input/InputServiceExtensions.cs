using System;

namespace MonoGameLibrary.Extensions.Input {
    /// <summary>
    /// Provides extension methods for <see cref="IInputService"/> to clarify 
    /// continuous vs. edge‑triggered input queries.
    /// </summary>
    public static class InputServiceExtensions {
        /// <summary>
        /// Returns true if the key is currently held down (continuous state).
        /// </summary>
        public static bool IsKeyHeld(this IInputService service, KeyCode code) {
            if (service == null) {
                throw new ArgumentNullException(nameof(service));
            }
            return service.IsKeyDown(code);
        }
        
        /// <summary>
        /// Returns true only on the frame the key was pressed (edge‑triggered).
        /// </summary>
        public static bool IsKeyPressed(this IInputService service, KeyCode code) {
            if (service == null) {
                throw new ArgumentNullException(nameof(service));
            }
            return service.WasKeyJustPressed(code);
        }
        
        /// <summary>
        /// Returns true if the button is currently held down (continuous state).
        /// </summary>
        public static bool IsButtonHeld(
            this IInputService service,
            PlayerIndex indexPlayer,
            GamePadButton button
        ) {
            if (service == null) {
                throw new ArgumentNullException(nameof(service));
            }
            return service.IsButtonDown(indexPlayer, button);
        }
        
        /// <summary>
        /// Returns true only on the frame the button was pressed (edge‑triggered).
        /// </summary>
        public static bool IsButtonPressed(
            this IInputService service,
            PlayerIndex indexPlayer,
            GamePadButton button
        ) {
            if (service == null) {
                throw new ArgumentNullException(nameof(service));
            }
            return service.WasButtonJustPressed(indexPlayer, button);
        }
    }
}
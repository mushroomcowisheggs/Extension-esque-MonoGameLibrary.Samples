using Microsoft.Xna.Framework;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Adapters.MonoGame {
    /// <summary>
    /// Extension methods for converting MonoGame <see cref="GameTime"/> to <see cref="FrameTime"/>.
    /// </summary>
    public static class GameTimeExtensions {
        /// <summary>
        /// Converts a MonoGame <see cref="GameTime"/> to a <see cref="FrameTime"/>. 
        /// </summary>
        /// <param name="timeGame">The MonoGame timing snapshot. </param>
        /// <returns>A new <see cref="FrameTime"/> initialized from <paramref name="timeGame"/>. </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="timeGame"/> is null. </exception>
        public static FrameTime ToFrameTime(this GameTime timeGame) {
            if (timeGame == null) { throw new System.ArgumentNullException(nameof(timeGame)); }
            return new FrameTime(timeGame.TotalGameTime, timeGame.ElapsedGameTime);
        }
    }
}
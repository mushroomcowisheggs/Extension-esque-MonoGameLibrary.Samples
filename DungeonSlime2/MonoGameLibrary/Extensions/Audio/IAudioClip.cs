using System;

namespace MonoGameLibrary.Extensions.Audio {
    /// <summary>
    /// Represents a short sound effect that can be played. 
    /// </summary>
    public interface IAudioClip {
        /// <summary>Gets the total duration of the clip. </summary>
        TimeSpan Duration { get; }
    }
}
using System;

namespace MonoGameLibrary.Extensions.Audio {
    /// <summary>
    /// Represents a music track (longer playback).
    /// </summary>
    public interface IAudioTrack {
        TimeSpan Duration { get; }
    }
}
using System;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Extensions.Audio;

namespace MonoGameLibrary.Adapters.MonoGame.Audio {
    /// <summary>
    /// Wraps a MonoGame <see cref="Song"/> as an <see cref="IAudioTrack"/>.
    /// </summary>
    public sealed class MonoGameAudioTrack : IAudioTrack {
        public Song Song { get; }
        
        public TimeSpan Duration {
            get {
                if (Song == null) {
                    return TimeSpan.Zero;
                }
                return Song.Duration;
            }
        }
        
        public MonoGameAudioTrack(Song song) {
            if (song == null) {
                throw new ArgumentNullException(nameof(song));
            }
            Song = song;
        }
    }
}
using System;
using Microsoft.Xna.Framework.Audio;
using MonoGameLibrary.Extensions.Audio;

namespace MonoGameLibrary.Adapters.MonoGame.Audio {
    /// <summary>
    /// Wraps a MonoGame <see cref="SoundEffect"/> as an <see cref="IAudioClip"/>.
    /// </summary>
    public sealed class MonoGameAudioClip : IAudioClip {
        public SoundEffect SoundEffect { get; }

        public TimeSpan Duration {
            get {
                if (SoundEffect == null) {
                    return TimeSpan.Zero;
                }
                return SoundEffect.Duration;
            }
        }

        public MonoGameAudioClip(SoundEffect effectSound) {
            if (effectSound == null) {
                throw new ArgumentNullException(nameof(effectSound));
            }
            SoundEffect = effectSound;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Audio;

namespace MonoGameLibrary.Adapters.MonoGame.Audio {
    /// <summary>
    /// Wraps MonoGame audio playback with simple global volume and mute support. 
    /// </summary>
    public sealed class AudioService : IAudioService, IDisposable {
        private readonly object _lock = new object();
        private readonly List<SoundEffectInstance> _listActiveSoundEffectInstances = new List<SoundEffectInstance>();
        private float _volumePreviousSong;
        private float _volumePreviousSoundEffect;
        private bool _flagDisposed;
        private bool _flagIsMuted;
        
        /// <summary>
        /// Gets a value that indicates whether audio is muted.
        /// </summary>
        public bool IsMuted {
            get { lock (_lock) { return _flagIsMuted; } }
            private set { lock (_lock) _flagIsMuted = value; }
        }
        
        /// <summary>
        /// Gets or sets the global volume for music. 
        /// </summary>
        public float SongVolume {
            get {
                if (IsMuted) {
                    return 0f;
                }
                
                return MediaPlayer.Volume;
            }
            set {
                if (IsMuted) {
                    return;
                }
                
                MediaPlayer.Volume = Math.Clamp(value, 0f, 1f);
            }
        }
        
        /// <summary>
        /// Gets or sets the global volume for sound effects. 
        /// </summary>
        public float SoundEffectVolume {
            get {
                if (IsMuted) {
                    return 0f;
                }
                
                return SoundEffect.MasterVolume;
            }
            set {
                if (IsMuted) {
                    return;
                }
                
                SoundEffect.MasterVolume = Math.Clamp(value, 0f, 1f);
            }
        }
        
        /// <summary>
        /// Creates a new audio controller. 
        /// </summary>
        public AudioService() {
        }
        
        /// <summary>
        /// Updates the audio controller, cleaning up finished sound effect instances.
        /// </summary>
        /// <param name="timeFrame">The frame timing information (unused).</param>
        public void Update(FrameTime timeFrame) {
            lock (_lock) {
                if (_flagDisposed) { return; }
                for (int i = _listActiveSoundEffectInstances.Count - 1; i >= 0; i -= 1) {
                    var instance = _listActiveSoundEffectInstances[i];
                    if (instance.State == SoundState.Stopped) {
                        instance.Dispose();
                        _listActiveSoundEffectInstances.RemoveAt(i);
                    }
                }
            }
        }
        
        /// <inheritdoc cref = "PlaySoundEffect(SoundEffect, float, float, float, bool)"/>
        /// <param name="effectSound">The sound effect to play. </param>
        /// <returns>The created sound effect instance. </returns>
        public SoundEffectInstance PlaySoundEffect(SoundEffect effectSound) {
            return PlaySoundEffect(effectSound, 1f, 0f, 0f, false);
        }

        /// <summary>
        /// Plays a sound effect. 
        /// </summary>
        /// <param name="effectSound">The sound effect to play. </param>
        /// <param name="volume">The volume, ranging from 0.0 (silence) to 1.0 (full). </param>
        /// <param name="pitch">The pitch adjustment, ranging from -1.0 (down an octave) to 0.0 (no change) to 1.0 (up an octave). </param>
        /// <param name="pan">The panning, ranging from -1.0 (left) to 0.0 (center) to 1.0 (right). </param>
        /// <param name="flagIsLooped">Whether the sound effect should loop after playback. </param>
        /// <returns>The created sound effect instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="effectSound"/> is null. </exception>
        public SoundEffectInstance PlaySoundEffect(SoundEffect effectSound, float volume, float pitch, float pan, bool flagIsLooped) {
            if (effectSound == null) {
                throw new ArgumentNullException(nameof(effectSound));
            }
            
            var instance = effectSound.CreateInstance();
            instance.Volume = Math.Clamp(volume, 0f, 1f);
            instance.Pitch = Math.Clamp(pitch, -1f, 1f);
            instance.Pan = Math.Clamp(pan, -1f, 1f);
            instance.IsLooped = flagIsLooped;
            instance.Play();
            lock (_lock) {
                _listActiveSoundEffectInstances.Add(instance);
            }
            return instance;
        }
        
        /// <summary>
        /// Toggles audio muting. 
        /// </summary>
        public void ToggleMute() {
            lock (_lock) {
                IsMuted = !IsMuted;
                if (IsMuted) {
                    _volumePreviousSong = MediaPlayer.Volume;
                    _volumePreviousSoundEffect = SoundEffect.MasterVolume;
                    MediaPlayer.Volume = 0f;
                    SoundEffect.MasterVolume = 0f;
                } else {
                    MediaPlayer.Volume = _volumePreviousSong;
                    SoundEffect.MasterVolume = _volumePreviousSoundEffect;
                }
            }
        }
        
        /// <inheritdoc />
        public void PlaySong(Song song, bool flagIsRepeating = true) {
            if (song == null) throw new ArgumentNullException(nameof(song));
            MediaPlayer.Stop();
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = flagIsRepeating;
        }
        
        /// <inheritdoc />
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            lock (_lock) {
                foreach (var instance in _listActiveSoundEffectInstances) {
                    instance.Dispose();
                }
                _listActiveSoundEffectInstances.Clear();
            }
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

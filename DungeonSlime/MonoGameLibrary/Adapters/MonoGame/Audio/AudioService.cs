using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.Audio;

namespace MonoGameLibrary.Adapters.MonoGame.Audio {
    /// <summary>
    /// MonoGame implementation of <see cref="IAudioService"/>. 
    /// </summary>
    public sealed class AudioService : IAudioService, IDisposable {
        private readonly object _lock = new object();
        private readonly IContentService _serviceContent;
        private readonly Dictionary<string, IAudioClip> _dictionaryClipCache;
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
            } set {
                if (IsMuted) {
                    return;
                }
                MediaPlayer.Volume = Math.Clamp(value, 0f, 1f);
            }
        }
        
        /// <summary>
        /// Gets or sets the global volume for sound effects (0.0 to 1.0). 
        /// Returns 0 when muted. 
        /// </summary>
        public float SoundEffectVolume {
            get {
                if (IsMuted) {
                    return 0f;
                }
                return SoundEffect.MasterVolume;
            } set {
                if (IsMuted) {
                    return;
                }
                SoundEffect.MasterVolume = Math.Clamp(value, 0f, 1f);
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioService"/> class. 
        /// </summary>
        /// <param name="serviceContent">
        /// Optional content service for loading clips by asset name. 
        /// If <c>null</c>, string-based loading methods will throw <see cref="NotSupportedException"/>. 
        /// </param>
        public AudioService(IContentService serviceContent = null) {
            _serviceContent = serviceContent;
            _dictionaryClipCache = new Dictionary<string, IAudioClip>();
            _listActiveSoundEffectInstances = new List<SoundEffectInstance>();
        }
        
        /// <inheritdoc />
        /// <param name="timeFrame">The frame timing information (unused).</param>
        public void Update(FrameTime timeFrame) {
            lock (_lock) {
                if (_flagDisposed) {
                    return;
                }
                for (int i = _listActiveSoundEffectInstances.Count - 1; i >= 0; i -= 1) {
                    var instance = _listActiveSoundEffectInstances[i];
                    if (instance.State == SoundState.Stopped) {
                        instance.Dispose();
                        _listActiveSoundEffectInstances.RemoveAt(i);
                    }
                }
            }
        }
        
        /// <inheritdoc />
        public void PlayAudioClip(IAudioClip clip, float volume = 1f, float pitch = 0f, float pan = 0f, bool flagLoop = false) {
            if (clip == null) {
                throw new ArgumentNullException(nameof(clip));
            }
            
            MonoGameAudioClip monoClip = clip as MonoGameAudioClip;
            if (monoClip == null) {
                throw new ArgumentException("Clip must be a MonoGameAudioClip.", nameof(clip));
            }
            
            SoundEffectInstance instance = monoClip.SoundEffect.CreateInstance();
            instance.Volume = Math.Clamp(volume, 0f, 1f);
            instance.Pitch = Math.Clamp(pitch, -1f, 1f);
            instance.Pan = Math.Clamp(pan, -1f, 1f);
            instance.IsLooped = flagLoop;
            instance.Play();
            
            lock (_lock) {
                _listActiveSoundEffectInstances.Add(instance);
            }
        }
        
        /// <inheritdoc />
        public void PlayAudioClip(string nameAsset, float volume = 1f, float pitch = 0f, float pan = 0f, bool loop = false) {
            if (string.IsNullOrWhiteSpace(nameAsset)) {
                throw new ArgumentException("Asset name cannot be empty.", nameof(nameAsset));
            }

            IAudioClip clip = LoadClip(nameAsset);
            PlayAudioClip(clip, volume, pitch, pan, loop);
        }
        
        /// <inheritdoc />
        public void PlayAudioTrack(IAudioTrack track, bool flagRepeat = true) {
            if (track == null) {
                throw new ArgumentNullException(nameof(track));
            }

            MonoGameAudioTrack trackMonoGameAudio = track as MonoGameAudioTrack;
            if (trackMonoGameAudio == null) {
                throw new ArgumentException("Track must be a MonoGameAudioTrack.", nameof(track));
            }

            MediaPlayer.Stop();
            MediaPlayer.Play(trackMonoGameAudio.Song);
            MediaPlayer.IsRepeating = flagRepeat;
        }
        
        /// <inheritdoc />
        public IAudioClip LoadClip(string nameAsset) {
            if (string.IsNullOrWhiteSpace(nameAsset)) {
                throw new ArgumentException("Asset name cannot be empty.", nameof(nameAsset));
            }
            
            if (_serviceContent == null) {
                throw new NotSupportedException(
                    "This AudioService instance was not configured with an IContentService. " +
                    "Use the constructor that accepts IContentService to enable string-based loading."
                );
            }
            
            lock (_lock) {
                if (_dictionaryClipCache.TryGetValue(nameAsset, out var cached)) {
                    return cached;
                }
                
                SoundEffect effect = _serviceContent.Load<SoundEffect>(nameAsset);
                var clip = new MonoGameAudioClip(effect);
                _dictionaryClipCache[nameAsset] = clip;
                return clip;
            }
        }
        
        /// <inheritdoc />
        public void ToggleMute() {
            lock (_lock) {
                _flagIsMuted = !_flagIsMuted;
                if (_flagIsMuted) {
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
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            lock (_lock) {
                foreach (var clip in _dictionaryClipCache.Values) {
                    if (clip is MonoGameAudioClip monoClip) {
                        monoClip.SoundEffect?.Dispose();
                    }
                }
                foreach (var instance in _listActiveSoundEffectInstances) {
                    instance.Dispose();
                }
                _listActiveSoundEffectInstances.Clear();
                _dictionaryClipCache.Clear();
            }
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
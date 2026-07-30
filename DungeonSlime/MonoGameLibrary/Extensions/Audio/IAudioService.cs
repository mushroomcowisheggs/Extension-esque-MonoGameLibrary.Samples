using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.Audio {
    /// <summary>
    /// Platform‑agnostic service for audio playback, volume control, and muting. 
    /// </summary>
    public interface IAudioService {
        /// <summary>Gets a value indicating whether audio is currently muted. </summary>
        bool IsMuted { get; }
        
        /// <summary>Gets or sets the global volume for music (0.0 to 1.0). </summary>
        float SongVolume { get; set; }
        
        /// <summary>Gets or sets the global volume for sound effects (0.0 to 1.0). </summary>
        float SoundEffectVolume { get; set; }
        
        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="clip">The audio clip to play. </param>
        /// <param name="volume">Volume (0.0 to 1.0). Default is 1.0. </param>
        /// <param name="pitch">Pitch adjustment (-1.0 to 1.0). Default is 0.0. </param>
        /// <param name="pan">Panning (-1.0 left to 1.0 right). Default is 0.0. </param>
        /// <param name="flagLoop">Whether the clip should loop. Default is false. </param>
        void PlayAudioClip(IAudioClip clip, float volume = 1f, float pitch = 0f, float pan = 0f, bool flagLoop = false);
        
        /// <summary>
        /// Plays a sound effect directly from its asset name.
        /// The clip is loaded and cached automatically.
        /// </summary>
        /// <param name="nameAsset">The asset name.</param>
        /// <param name="volume">Volume (0.0 to 1.0).</param>
        /// <param name="pitch">Pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">Panning (-1.0 left to 1.0 right).</param>
        /// <param name="flagShouldLoop">Whether the sound should loop.</param>
        void PlayAudioClip(string nameAsset, float volume = 1f, float pitch = 0f, float pan = 0f, bool flagShouldLoop = false);
        
        /// <summary>
        /// Plays a music track, stopping any currently playing track. 
        /// </summary>
        /// <param name="track">The music track to play.</param>
        /// <param name="flagRepeat">Whether the track should repeat. Default is true. </param>
        void PlayAudioTrack(IAudioTrack track, bool flagRepeat = true);
        
        /// <summary>
        /// Loads an audio clip by asset name. The clip is cached internally.
        /// </summary>
        /// <param name="nameAsset">The asset name (relative to content root).</param>
        /// <returns>The loaded audio clip.</returns>
        /// <exception cref="NotSupportedException">If the service does not support string-based loading.</exception>
        IAudioClip LoadClip(string nameAsset);
        
        /// <summary>
        /// Toggles mute state (on/off). 
        /// </summary>
        void ToggleMute();
        
        /// <summary>
        /// Updates the audio controller, cleaning up finished sound effect instances. 
        /// Called each frame to perform maintenance (e.g., cleaning up finished instances). 
        /// </summary>
        /// <param name="timeFrame">The frame timing information (unused). </param>
        void Update(FrameTime timeFrame);
    }
}
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.MonoGame.Audio {
    /// <summary>
    /// Provides audio playback and volume control services.
    /// </summary>
    public interface IAudioService {
        /// <summary>
        /// Gets a value indicating whether audio is muted.
        /// </summary>
        bool IsMuted { get; }
        
        /// <summary>
        /// Gets or sets the global music volume (0.0 to 1.0).
        /// </summary>
        float SongVolume { get; set; }
        
        /// <summary>
        /// Gets or sets the global sound effect volume (0.0 to 1.0).
        /// </summary>
        float SoundEffectVolume { get; set; }
        
        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="effectSound">The sound effect to play.</param>
        /// <returns>The created <see cref="SoundEffectInstance"/> for control.</returns>
        SoundEffectInstance PlaySoundEffect(SoundEffect effectSound);
        
        /// <summary>
        /// Plays a sound effect with full control over volume, pitch, pan, and looping.
        /// </summary>
        /// <param name="effectSound">The sound effect to play.</param>
        /// <param name="volume">Volume (0.0 to 1.0).</param>
        /// <param name="pitch">Pitch adjustment (-1.0 to 1.0).</param>
        /// <param name="pan">Panning (-1.0 left to 1.0 right).</param>
        /// <param name="flagIsLooped">Whether to loop the sound.</param>
        /// <returns>The created <see cref="SoundEffectInstance"/>.</returns>
        SoundEffectInstance PlaySoundEffect(SoundEffect effectSound, float volume, float pitch, float pan, bool flagIsLooped);
        
        /// <summary>
        /// Toggles mute state.
        /// </summary>
        void ToggleMute();
        
        /// <summary>
        /// Plays a song, stopping any currently playing song. 
        /// </summary>
        /// <param name="song">The song to play.</param>
        /// <param name="flagIsRepeating">Whether the song should loop. Default is true.</param>
        void PlaySong(Song song, bool flagIsRepeating = true);
        
        /// <summary>
        /// Performs internal maintenance (e.g., cleaning up finished sound instances).
        /// Called by the host module each frame.
        /// </summary>
        void Update(FrameTime timeFrame);
    }
}
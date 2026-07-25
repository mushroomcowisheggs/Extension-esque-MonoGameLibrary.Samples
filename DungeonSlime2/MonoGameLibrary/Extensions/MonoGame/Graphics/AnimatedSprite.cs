using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.MonoGame.Graphics {
    /// <summary>
    /// A sprite that automatically advances through an animation. 
    /// </summary>
    public sealed class AnimatedSprite : Sprite {
        private int _frameCurrent;
        private TimeSpan _elapsed;

        /// <summary>
        /// Gets or sets the animation to play. 
        /// </summary>
        public Animation Animation { get; set; }

        /// <summary>
        /// Creates a new animated sprite. 
        /// </summary>
        public AnimatedSprite() {
        }

        /// <summary>
        /// Creates a new animated sprite for the provided animation. 
        /// </summary>
        public AnimatedSprite(Animation animation) {
            Animation = animation;
            if (animation != null && animation.Frames != null && animation.Frames.Count > 0) {
                Region = animation.Frames[0];
            }
        }

        /// <summary>
        /// Advances the animation by the given elapsed time. 
        /// </summary>
        /// <param name="timeGame">A snapshot of the timing values for the current frame. </param>
        public void Update(GameTime timeGame) {
            if (Animation == null || Animation.Frames.Count <= 1) {
                return;
            }

            _elapsed += timeGame.ElapsedGameTime;
            if (_elapsed >= Animation.Delay) {
                _elapsed = TimeSpan.Zero;
                _frameCurrent = (_frameCurrent + 1) % Animation.Frames.Count;
                Region = Animation.Frames[_frameCurrent];
            }
        }

        /// <summary>
        /// Advances the animation by the current frame time. 
        /// </summary>
        /// <param name="timeFrame">A snapshot of the timing values for the current frame. </param>
        public void Update(FrameTime timeFrame) {
            if (Animation == null || Animation.Frames.Count <= 1) {
                return;
            }

            _elapsed += timeFrame.DeltaTimeSpan;
            if (_elapsed >= Animation.Delay) {
                _elapsed = TimeSpan.Zero;
                _frameCurrent = (_frameCurrent + 1) % Animation.Frames.Count;
                Region = Animation.Frames[_frameCurrent];
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Extensions.MonoGame.Graphics {
    /// <summary>
    /// Represents a collection of texture regions displayed over time. 
    /// </summary>
    public sealed class Animation {
        /// <summary>
        /// Gets or sets the frames that make up the animation. 
        /// </summary>
        public List<TextureRegion> Frames { get; set; } = new List<TextureRegion>();

        /// <summary>
        /// Gets or sets the duration between frames. 
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(100);
    }
}

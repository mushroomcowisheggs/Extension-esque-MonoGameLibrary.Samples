using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Concurrency;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.MonoGame.Audio {
    /// <summary>
    /// A host-driven module that updates the audio controller each frame.
    /// </summary>
    public sealed class AudioModule : IUpdateable, IDisposable {
        private readonly IAudioService _serviceAudio;
        private readonly ILogger _logger;
        private readonly object _lock = new object();
        private bool _flagEnabled = true;
        private bool _flagDisposed = false;
        
        /// <summary>
        /// Gets the update order.
        /// </summary>
        public int Order { get; } = 0;
        
        /// <summary>
        /// Gets or sets whether the module updates.
        /// </summary>
        public bool Enabled {
            get { lock (_lock) { return _flagEnabled; } }
            set { lock (_lock) _flagEnabled = value; }
        }
        
        /// <summary>
        /// Creates a new audio module.
        /// </summary>
        public AudioModule(IAudioService serviceAudio, Optional<ILogger> logger = default) {
            if (serviceAudio == null) {
                throw new ArgumentNullException(nameof(serviceAudio));
            }
            _serviceAudio = serviceAudio;
            _logger = logger.HasValue ? logger.Value : NullLogger.Instance;
        }
        
        /// <inheritdoc />
        public void Update(FrameTime timeFrame) {
            bool flagShouldUpdate;
            lock (_lock) {
                flagShouldUpdate = _flagEnabled && !_flagDisposed;
            }
            if (!flagShouldUpdate) { return; }

            _serviceAudio.Update(timeFrame);
        }

        /// <summary>
        /// Disposes the audio module and its controller.
        /// </summary>
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

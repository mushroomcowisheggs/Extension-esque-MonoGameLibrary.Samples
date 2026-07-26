using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Concurrency;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.Audio {
    /// <summary>
    /// Host-driven module that forwards update calls to <see cref="IAudioService"/>.
    /// Contains no platform logic.
    /// </summary>
    public sealed class AudioModule : IUpdateable, IDisposable {
        private readonly IAudioService _serviceAudio;
        private readonly ILogger _logger;
        private readonly object _lock = new object();
        private bool _flagEnabled = true;
        private bool _flagDisposed = false;
        
        /// <summary>
        /// Gets the update order. Default is 0.
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
        /// <param name="serviceAudio">The audio service to forward calls to.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceAudio"/> is null.</exception>
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
            if (!flagShouldUpdate) {
                return;
            }
            
            _serviceAudio.Update(timeFrame);
        }
        
        /// <summary>
        /// Disposes the module (no unmanaged resources). 
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
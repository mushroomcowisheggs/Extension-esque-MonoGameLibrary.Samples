using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.MonoGame.Input {
    /// <summary>
    /// A host-driven module that updates the input service each frame. 
    /// </summary>
    public sealed class InputModule : IUpdateable, IDisposable {
        private readonly IInputService _serviceInput;
        private readonly ILogger _logger;
        private readonly object _lock = new object();
        private bool _flagEnabled = true;
        private bool _flagDisposed = false;
        
        /// <summary>
        /// Gets the update order. Input updates before most gameplay systems. 
        /// </summary>
        public int Order { get; } = -100;
        
        /// <summary>
        /// Gets or sets whether the module updates. 
        /// </summary>
        public bool Enabled {
            get { lock (_lock) { return _flagEnabled; } }
            set { lock (_lock) _flagEnabled = value; }
        }
        
        /// <summary>
        /// Creates a new input module. 
        /// </summary>
        public InputModule(IInputService serviceInput, Optional<ILogger> logger = default) {
            if (serviceInput == null) {
                throw new ArgumentNullException(nameof(serviceInput));
            }
            
            _serviceInput = serviceInput;
            
            _logger = logger.HasValue ? logger.Value : NullLogger.Instance;
        }
        
        /// <inheritdoc />
        public void Update(FrameTime timeFrame) {
            bool flagShouldUpdate;
            lock (_lock) {
                flagShouldUpdate = _flagEnabled && !_flagDisposed;
            }
            if (!flagShouldUpdate) { return; }
            
            _serviceInput.Update(timeFrame);
        }
        
        /// <summary>
        /// Disposes the input module (no unmanaged resources). 
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

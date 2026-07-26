using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.Input {
    /// <summary>
    /// Host-driven module that forwards update calls to <see cref="IInputService"/>.
    /// Contains no platform logic.
    /// </summary>
    public sealed class InputModule : IUpdateable, IDisposable {
        private readonly IInputService _serviceInput;
        private readonly ILogger _logger;
        private readonly object _lock = new object();
        private bool _flagEnabled = true;
        private bool _flagDisposed = false;
        
        /// <summary>
        /// Gets the update order. Input should update before most systems, so default is -100.
        /// </summary>
        public int Order { get; } = -64;
        
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
        /// <param name="serviceInput">The input service to forward calls to.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceInput"/> is null.</exception>
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
            if (!flagShouldUpdate) {
                return;
            }
            
            _serviceInput.Update(timeFrame);
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
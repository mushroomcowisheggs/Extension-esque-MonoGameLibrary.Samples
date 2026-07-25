using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Adapters.Gum;

namespace MonoGameLibrary.Extensions.Gum {
    /// <summary>
    /// Host-driven module that updates and draws the Gum UI system.
    /// </summary>
    public sealed class GumModule : global::MonoGameLibrary.Core.Lifecycle.IUpdateable, global::MonoGameLibrary.Core.Lifecycle.IDrawable, IDisposable {
        private readonly IGumService _serviceGum;
        private readonly int _order;
        private bool _flagEnabled = true;
        private bool _flagVisible = true;
        private bool _flagDisposed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GumModule"/> class.
        /// </summary>
        /// <param name="serviceGum">The Gum service to drive.</param>
        /// <param name="order">Execution order (default 0).</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceGum"/> is null.</exception>
        public GumModule(IGumService serviceGum, int order = 0) {
            if (serviceGum == null) {
                throw new ArgumentNullException(nameof(serviceGum));
            }
            _serviceGum = serviceGum;
            _order = order;
        }
        
        /// <inheritdoc />
        public int Order { get { return _order; } }
        
        /// <inheritdoc />
        public bool Enabled {
            get { return _flagEnabled; }
            set { _flagEnabled = value; }
        }

        /// <inheritdoc />
        public bool Visible {
            get { return _flagVisible; }
            set { _flagVisible = value; }
        }

        /// <inheritdoc />
        public void Update(FrameTime timeFrame) {
            if (!_flagEnabled || _flagDisposed) {
                return;
            }
            
            // IGumService.Update expects GameTime, not FrameTime.
            // You need to convert FrameTime to GameTime.
            // Since FrameTime is a Core struct, you'll need to create a GameTime wrapper.
            // A simple approach: use the total and delta times.
            var timeGame = new GameTime(
                timeFrame.TotalTimeSpan,
                timeFrame.DeltaTimeSpan
            );
            _serviceGum.Update(timeGame);
        }

        /// <inheritdoc />
        public void Draw(FrameTime timeFrame, IRenderContext contextRender) {
            if (!_flagVisible || _flagDisposed) {
                return;
            }
            
            _serviceGum.Draw();
        }
        
        /// <summary>
        /// Disposes the module (no unmanaged resources).
        /// </summary>
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            _flagDisposed = true;
            // The service itself may be disposed elsewhere; do not dispose it here if shared.
            GC.SuppressFinalize(this);
        }
    }
}
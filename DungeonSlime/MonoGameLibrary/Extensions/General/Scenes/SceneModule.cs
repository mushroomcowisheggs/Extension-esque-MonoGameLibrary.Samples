using System;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.General.Scenes {
    /// <summary>
    /// A module that wraps <see cref="ISceneService"/> and forwards lifecycle calls
    /// from the host to the service. Implements <see cref="IUpdateable"/> and <see cref="IDrawable"/>.
    /// </summary>
    public sealed class SceneModule : IUpdateable, IDrawable {
        private readonly ISceneService _service;
        private readonly int _order;
        private bool _flagEnabled = true;
        private bool _flagVisible = true;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneModule"/> class.
        /// </summary>
        /// <param name="service">The scene service to forward calls to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is null.</exception>
        public SceneModule(ISceneService service, int order = 0) {
            if (service == null) {
                throw new ArgumentNullException(nameof(service));
            }
            
            _service = service;
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
            if (!Enabled) {
                return;
            }
            
            _service.Update(timeFrame);
        }
        
        /// <inheritdoc />
        public void Draw(FrameTime timeFrame, IRenderContext contextRender) {
            if (!Visible) {
                return;
            }
            
            _service.Draw(timeFrame, contextRender);
        }
    }
}
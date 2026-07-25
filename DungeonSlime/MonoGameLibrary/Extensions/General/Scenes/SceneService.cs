using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.General.Scenes {
    /// <summary>
    /// Implements the scene management logic: switching, updating, and drawing the active scene. 
    /// This class is platform-agnostic and does not implement lifecycle interfaces. 
    /// </summary>
    public sealed class SceneService : ISceneService, IDisposable {
        private readonly object _lock = new object();
        private readonly IContentService _serviceContent;
        private readonly ILogger _logger;
        private readonly Optional<IProfiler> _profiler;
        private readonly Optional<IContentServiceFactory> _factoryContent;
        private Scene _sceneCurrent;
        private Scene _scenePending;
        private bool _flagDisposed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneService"/> class 
        /// without automatic content factory support. 
        /// </summary>
        /// <param name="serviceContent">Content service used by scenes (for factory overload). </param>
        /// <param name="logger">Logger for diagnostic output.</param>
        /// <param name="profiler">Optional profiler for performance measurements. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceContent"/> is null. </exception>
        public SceneService(IContentService serviceContent, Optional<ILogger> logger = default, Optional<IProfiler> profiler = default) {
            if (serviceContent == null) {
                throw new ArgumentNullException(nameof(serviceContent));
            }
            
            _serviceContent = serviceContent;
            _logger = logger.HasValue ? logger.Value : NullLogger.Instance;
            _profiler = profiler;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneService"/> class 
        /// with an optional content factory for automatic resource management. 
        /// </summary>
        /// <param name="serviceContent">Content service used for compatibility (may be shared). </param>
        /// <param name="factoryContent">Factory used to create new content services for scenes when using the factory-based switch method. </param>
        /// <param name="logger">Logger for diagnostic output. </param>
        /// <param name="profiler">Optional profiler for performance measurements. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceContent"/> is null. </exception>
        public SceneService(IContentService serviceContent, Optional<IContentServiceFactory> factoryContent, Optional<ILogger> logger = default, Optional<IProfiler> profiler = default) {
            if (serviceContent == null)
                throw new ArgumentNullException(nameof(serviceContent));

            _serviceContent = serviceContent;
            _factoryContent = factoryContent;
            _logger = logger.HasValue ? logger.Value : NullLogger.Instance;
            _profiler = profiler;
        }
        
        /// <inheritdoc />
        public Scene CurrentScene {
            get { lock (_lock) { return _sceneCurrent; } }
        }
        
        /// <inheritdoc />
        public void ChangeScene(Scene scene) {
            if (scene == null) {
                throw new ArgumentNullException(nameof(scene));
            }
            lock (_lock) {
                _scenePending = scene;
            }
        }

        /// <inheritdoc />
        public void ChangeScene(Func<IContentService, Scene> factory) {
            if (factory == null) {
                throw new ArgumentNullException(nameof(factory));
            }
            if (!_factoryContent.HasValue) {
                throw new InvalidOperationException("No IContentServiceFactory registered. Cannot auto-create content.");
            }
            
            IContentService contentNew = _factoryContent.Value.Create();
            Scene sceneNew = factory(contentNew);
            if (sceneNew == null) {
                throw new InvalidOperationException("The scene factory returned null.");
            }
            
            ChangeScene(sceneNew);
        }
        
        /// <inheritdoc />
        public void Update(FrameTime timeFrame) {
            if (_flagDisposed) {
                return;
            }
            
            Scene sceneToActivate = null;
            lock (_lock) {
                if (_scenePending != null) {
                    sceneToActivate = _scenePending;
                    _scenePending = null;
                }
            }
            
            if (sceneToActivate != null) {
                if (_sceneCurrent != null) {
                    _sceneCurrent.Dispose();
                }
                _sceneCurrent = sceneToActivate;
                _sceneCurrent.LoadContent();
                _sceneCurrent.Initialize();
            }
            
            if (_sceneCurrent != null && _sceneCurrent.Enabled) {
                _sceneCurrent.Update(timeFrame);
            }
        }
        
        /// <inheritdoc />
        public void Draw(FrameTime timeFrame, IRenderContext contextRender) {
            if (_flagDisposed) {
                return;
            }
            
            if (_sceneCurrent != null && _sceneCurrent.Visible) {
                _sceneCurrent.Draw(timeFrame, contextRender);
            }
        }
        
        /// <summary>
        /// Disposes the service and any active or pending scenes. 
        /// </summary>
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }
            
            if (_sceneCurrent != null) {
                _sceneCurrent.Dispose();
            }
            
            if (_scenePending != null) {
                _scenePending.Dispose();
            }
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

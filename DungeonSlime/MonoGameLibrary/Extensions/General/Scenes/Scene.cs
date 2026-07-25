using System;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Extensions.General.Scenes {
    /// <summary>
    /// Base class for game scenes. Lifecycle is managed by <see cref="SceneService"/>. 
    /// </summary>
    public abstract class Scene : IDisposable {
        private bool _flagDisposed;
        
        /// <summary>
        /// Gets the content service used by the scene. 
        /// </summary>
        protected IContentService ContentService { get; }
        
        /// <summary>
        /// Gets the logger used by the scene. 
        /// </summary>
        protected ILogger Logger { get; }
        
        /// <summary>
        /// Gets the optional profiler for the scene. 
        /// </summary>
        protected Optional<IProfiler> Profiler { get; }
        
        /// <summary>
        /// Gets the update order. 
        /// </summary>
        public int Order { get; }
        
        /// <summary>
        /// Gets or sets whether the scene updates. 
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Gets or sets whether the scene draws. 
        /// </summary>
        public bool Visible { get; set; } = true;
        
        /// <summary>
        /// Creates a new scene with default ordering. 
        /// </summary>
        /// <param name="serviceContent">The content service used to load assets for this scene. </param>
        /// <param name="logger">Optional logger for diagnostic output. If not provided, a <see cref="NullLogger"/> is used. </param>
        /// <param name="profiler">Optional profiler for performance measurements. If not provided, no profiling is performed. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceContent"/> is null. </exception>
        protected Scene(IContentService serviceContent, Optional<ILogger> logger = default, Optional<IProfiler> profiler = default)
        : this(serviceContent, 0, logger, profiler) {
        }
        
        /// <summary>
        /// Creates a new scene with an explicit ordering. 
        /// </summary>
        /// <param name="serviceContent">The content service used to load assets for this scene. </param>
        /// <param name="order">The lifecycle execution order of the scene. Lower values execute earlier. </param>
        /// <param name="logger">Optional logger for diagnostic output. If not provided, a <see cref="NullLogger"/> is used. </param>
        /// <param name="profiler">Optional profiler for performance measurements. If not provided, no profiling is performed. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceContent"/> is null. </exception>
        protected Scene(IContentService serviceContent, int order, Optional<ILogger> logger = default, Optional<IProfiler> profiler = default) {
            if (serviceContent == null) {
                throw new ArgumentNullException(nameof(serviceContent));
            }
            
            ContentService = serviceContent;
            Logger = logger.HasValue ? logger.Value : NullLogger.Instance;
            Profiler = profiler;
            Order = order;
        }
        
        /// <summary>
        /// Called once to load content. Override to load textures, fonts, sounds, etc. 
        /// </summary>
        public virtual void LoadContent() {
        }
        
        /// <summary>
        /// Called after <see cref="LoadContent"/> to perform setup that depends on loaded assets. 
        /// Override to initialize UI, calculate positions, etc. 
        /// </summary>
        public virtual void Initialize() {
        }
        
        /// <summary>
        /// Called each frame to update the scene logic. 
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame. </param>
        public virtual void Update(FrameTime timeFrame) {
        }
        
        /// <summary>
        /// Called each frame to draw the scene. 
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame. </param>
        /// <param name="contextRender">The platform-specific rendering context. </param>
        public virtual void Draw(FrameTime timeFrame, IRenderContext contextRender) {
        }
        
        /// <summary>
        /// Disposes the scene and releases resources. 
        /// </summary>
        public virtual void Dispose() {
            if (_flagDisposed) {
                return;
            }
            
            _flagDisposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Override to release managed resources. 
        /// </summary>
        /// <param name="flagDisposing">True if called from Dispose; false if from finalizer.</param>
        protected virtual void Dispose(bool flagDisposing) {
            if (_flagDisposed) { return; }
            if (flagDisposing) {
                // Ensure that all scene-specific resources are unloaded
                if (ContentService is IDisposable contentDisposable) {
                    contentDisposable.Dispose();
                }
            }
            
            _flagDisposed = true;
        }
    }
}

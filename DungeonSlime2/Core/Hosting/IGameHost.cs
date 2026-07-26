using System;

namespace MonoGameLibrary.Core.Hosting {
    /// <summary>
    /// Represents a game host that manages the lifecycle of game modules. 
    /// </summary>
    public interface IGameHost : IDisposable {
        /// <summary>
        /// Gets or sets a callback that is invoked when an unhandled exception occurs in any module. 
        /// If not set, exceptions are rethrown. 
        /// </summary>
        Action<Exception, string> OnError { get; set; }
        IServiceRegistry Services { get; }
        void AddModule(object module);
        void Initialize(IContentService serviceContent);
        void Update(FrameTime timeFrame);
        void Draw(FrameTime timeFrame, IRenderContext contextRender);
    }
}
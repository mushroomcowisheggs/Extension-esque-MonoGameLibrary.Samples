using System;
using MonoGameLibrary.Core.Hosting;

namespace MonoGameLibrary.Extensions.General.Scenes {
    /// <summary>
    /// Provides scene switching and lifecycle orchestration for the active scene. 
    /// </summary>
    public interface ISceneService {
        /// <summary>
        /// Gets the currently active scene. 
        /// </summary>
        Scene CurrentScene { get; }

        /// <summary>
        /// Switches to the specified scene. 
        /// </summary>
        /// <param name="scene">The scene instance to switch to. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scene"/> is null. </exception>
        void ChangeScene(Scene scene);
        
        /// <summary>
        /// Switches to a scene created by the provided factory.
        /// The factory receives a new <see cref="IContentService"/> instance,
        /// which will be automatically disposed when the scene is replaced.
        /// </summary>
        /// <param name="factory">A delegate that creates a <see cref="Scene"/> given a content service.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no <see cref="IContentServiceFactory"/> is registered.</exception>
        void ChangeScene(Func<IContentService, Scene> factory);
        
        /// <summary>
        /// Updates the active scene's logic. Called by the module wrapper each frame.
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame.</param>
        void Update(FrameTime timeFrame);
        
        /// <summary>
        /// Draws the active scene. Called by the module wrapper each frame.
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame.</param>
        /// <param name="contextRender">The platform-specific rendering context.</param>
        void Draw(FrameTime timeFrame, IRenderContext contextRender);
    }
}

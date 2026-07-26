using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Core.Lifecycle {
    /// <summary>
    /// Implemented by modules that need to be drawn every frame.
    /// </summary>
    public interface IDrawable {
        /// <summary>
        /// Called once per frame to draw the module.
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame.</param>
        /// <param name="contextRender">The platform-specific rendering context.</param>
        void Draw(FrameTime timeFrame, IRenderContext contextRender);

        /// <summary>
        /// Gets the draw order. Lower values draw first.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Gets whether the module is visible. If <c>false</c>, <see cref="Draw"/> is skipped.
        /// </summary>
        bool Visible { get; }
    }
}
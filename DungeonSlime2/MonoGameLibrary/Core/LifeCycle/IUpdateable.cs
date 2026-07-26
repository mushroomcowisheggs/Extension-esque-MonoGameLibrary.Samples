using MonoGameLibrary.Core.Time;

namespace MonoGameLibrary.Core.Lifecycle {
    /// <summary>
    /// Implemented by modules that need to be updated every frame.
    /// </summary>
    public interface IUpdateable {
        /// <summary>
        /// Called once per frame to update the module's logic.
        /// </summary>
        /// <param name="timeFrame">Timing information for the current frame.</param>
        void Update(FrameTime timeFrame);

        /// <summary>
        /// Gets the update order. Lower values update first.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Gets whether the module is enabled for updating. If <c>false</c>, <see cref="Update"/> is skipped.
        /// </summary>
        bool Enabled { get; }
    }
}
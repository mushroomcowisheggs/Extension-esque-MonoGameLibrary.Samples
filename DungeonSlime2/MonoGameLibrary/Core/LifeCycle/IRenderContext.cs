namespace MonoGameLibrary.Core.Lifecycle {
    /// <summary>
    /// Represents a platform-specific rendering context. Provides access to the
    /// native rendering object (e.g., SpriteBatch for MonoGame).
    /// </summary>
    public interface IRenderContext {
        /// <summary>
        /// Gets the underlying native rendering object. This must be cast to the
        /// expected type in platform-specific modules.
        /// </summary>
        object NativeContext { get; }
    }
}
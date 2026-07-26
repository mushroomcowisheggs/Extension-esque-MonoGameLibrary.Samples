using MonoGameLibrary.Core.Hosting;

namespace MonoGameLibrary.Core.Lifecycle {
    /// <summary>
    /// Implemented by modules that require a one-time content loading phase.
    /// Modules must obtain necessary services (like <see cref="IContentService"/>) 
    /// through constructor injection or <see cref="IServiceRegistry"/>.
    /// </summary>
    public interface ILoadable {
        /// <summary>
        /// Called when the module should load its content.
        /// </summary>
        void LoadContent();
    }
}
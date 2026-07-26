using MonoGameLibrary.Extensions.Input;

namespace MonoGameLibrary.Extensions.UserInterface {
    /// <summary>
    /// Optional capability interface for UI services that support custom Tab navigation key configuration.
    /// </summary>
    public interface ITabNavigationSupport {
        /// <summary>
        /// Adds a key that triggers forward Tab navigation.
        /// </summary>
        /// <param name="codeKey">The key to add.</param>
        void AddTabForwardKey(KeyCode codeKey);
        
        /// <summary>
        /// Adds a key that triggers reverse Tab navigation.
        /// </summary>
        /// <param name="codeKey">The key to add.</param>
        void AddTabReverseKey(KeyCode codeKey);
    }
}
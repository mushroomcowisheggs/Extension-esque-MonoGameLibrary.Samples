using System;

namespace MonoGameLibrary.Core.Hosting {
    /// <summary>
    /// Provides a factory method for creating new <see cref="IContentService"/> instances. 
    /// This is used to give each scene its own independent content manager for resource isolation. 
    /// </summary>
    public interface IContentServiceFactory {
        /// <summary>
        /// Creates a new content service instance. 
        /// The returned instance is independent and should be disposed when no longer needed. 
        /// </summary>
        /// <returns>A new <see cref="IContentService"/>. </returns>
        IContentService Create();
    }
}
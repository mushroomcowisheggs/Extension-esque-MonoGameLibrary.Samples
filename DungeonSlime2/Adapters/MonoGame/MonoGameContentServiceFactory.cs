using System;
using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Core.Hosting;

namespace MonoGameLibrary.Adapters.MonoGame {
    /// <summary>
    /// MonoGame implementation of <see cref="IContentServiceFactory"/>. 
    /// Each call to <see cref="Create"/> returns a new <see cref="MonoGameContentService"/> 
    /// backed by a fresh <see cref="ContentManager"/>. 
    /// </summary>
    public sealed class MonoGameContentServiceFactory : IContentServiceFactory {
        private readonly IServiceProvider _providerService;
        private readonly string _directoryRoot;

        /// <summary>
        /// Initializes a new factory instance. 
        /// </summary>
        /// <param name="serviceProvider">The service provider used by the ContentManager. </param>
        /// <param name="rootDirectory">The root directory for content (e.g., "Content"). </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is null. </exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rootDirectory"/> is null or whitespace. </exception>
        public MonoGameContentServiceFactory(IServiceProvider providerService, string directoryRoot) {
            if (providerService == null) {
                throw new ArgumentNullException(nameof(providerService));
            }
            if (string.IsNullOrWhiteSpace(directoryRoot)) {
                throw new ArgumentException("Root directory cannot be empty.", nameof(directoryRoot));
            }
            
            _providerService = providerService;
            _directoryRoot = directoryRoot;
        }
        
        /// <inheritdoc />
        public IContentService Create() {
            var managerContent = new ContentManager(_providerService) {
                RootDirectory = _directoryRoot
            };
            return new MonoGameContentService(managerContent);
        }
    }
}
using System;
using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Core.Hosting;

namespace MonoGameLibrary.Adapters.MonoGame {
    /// <summary>
    /// A MonoGame implementation of <see cref="IContentService"/>. 
    /// </summary>
    public sealed class MonoGameContentService : IContentService {
        private readonly ContentManager _managerContent;
        private bool _flagDisposed;

        /// <summary>
        /// Creates a content service backed by a MonoGame content manager. 
        /// </summary>
        public MonoGameContentService(ContentManager managerContent) {
            if (managerContent == null) { throw new ArgumentNullException(nameof(managerContent)); }
            _managerContent = managerContent;
        }

        /// <summary>
        /// Gets the underlying MonoGame ContentManager used by this service.
        /// </summary>
        public ContentManager ContentManager {
            get { return _managerContent; }
        }
        
        /// <inheritdoc />
        public T Load<T>(string nameAsset) {
            return _managerContent.Load<T>(nameAsset);
        }

        /// <inheritdoc />
        public void Unload() {
            _managerContent.Unload();
        }

        /// <inheritdoc />
        public void Dispose() {
            if (_flagDisposed) {
                return;
            }

            _flagDisposed = true;
            try {
                Unload();
            } finally {
                _managerContent.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}

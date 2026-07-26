using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Core.Lifecycle;

namespace MonoGameLibrary.Adapters.MonoGame {
    /// <summary>
    /// Adapts a MonoGame <see cref="Game"/> to work with an <see cref="IGameHost"/>. 
    /// The caller is responsible for calling <see cref="SpriteBatch.Begin"/> before 
    /// <see cref="Draw"/> and <see cref="SpriteBatch.End"/> afterwards. 
    /// </summary>
    public class MonoGameAdapter : IDisposable {
        private readonly IGameHost _host;
        private readonly SpriteBatch _batchSprite;
        private bool _flagDisposed = false;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameAdapter"/> class. 
        /// </summary>
        /// <param name="host">The <see cref="IGameHost"/> to drive. </param>
        /// <param name="batchSprite">The <see cref="SpriteBatch"/> used for drawing. </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="host"/> or <paramref name="batchSprite"/> is null. 
        /// </exception>
        public MonoGameAdapter(IGameHost host, SpriteBatch batchSprite) {
            if (host == null) { throw new ArgumentNullException(nameof(host)); }
            _host = host;
            if (batchSprite == null) { throw new ArgumentNullException(nameof(batchSprite)); }
            _batchSprite = batchSprite;
        }
        
        /// <summary>
        /// Loads all module content using the provided <see cref="IContentService"/>. 
        /// </summary>
        /// <param name="serviceContent">The content service to use for loading. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceContent"/> is null. </exception>
        public void LoadContent(IContentService serviceContent) {
            if (serviceContent == null) { throw new ArgumentNullException(nameof(serviceContent)); }
            _host.Initialize(serviceContent);
        }
        
        /// <summary>
        /// Updates all modules for the current frame. 
        /// </summary>
        /// <param name="timeGame">The MonoGame timing snapshot. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="timeGame"/> is null. </exception>
        public void Update(GameTime timeGame) {
            if (timeGame == null) { throw new ArgumentNullException(nameof(timeGame)); }
            _host.Update(timeGame.ToFrameTime());
        }
        
        /// <summary>
        /// Draws all modules. The caller must have called <see cref="SpriteBatch.Begin"/> 
        /// before invoking this method, and <see cref="SpriteBatch.End"/> afterwards. 
        /// </summary>
        /// <param name="timeGame">The MonoGame timing snapshot. </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="timeGame"/> is null. </exception>
        public void Draw(GameTime timeGame) {
            if (timeGame == null) { throw new ArgumentNullException(nameof(timeGame)); }
            var context = new MonoGameRenderContext(_batchSprite);
            _host.Draw(timeGame.ToFrameTime(), context);
        }
        
        /// <summary>
        /// Disposes the adapter and the underlying host. 
        /// </summary>
        public void Dispose() {
            if (_flagDisposed) { return; }
            _host.Dispose();
            _flagDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
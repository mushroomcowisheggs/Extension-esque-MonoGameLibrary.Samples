using System;
using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Core.Lifecycle;

namespace MonoGameLibrary.Adapters.Gum {
    /// <summary>
    /// Module that initializes Gum during the content loading phase.
    /// </summary>
    internal sealed class GumInitializationModule : ILoadable {
        private readonly IGumService _serviceGum;
        private readonly ContentManager _managerContent;
        
        public GumInitializationModule(IGumService serviceGum, ContentManager managerContent) {
            if (serviceGum == null) {
                throw new ArgumentNullException(nameof(serviceGum));
            }
            if (managerContent == null) {
                throw new ArgumentNullException(nameof(managerContent));
            }
            _serviceGum = serviceGum;
            _managerContent = managerContent;
        }
        
        public void LoadContent() {
            _serviceGum.Initialize();
            global::MonoGameGum.GumService.Default.ContentLoader.XnaContentManager = _managerContent;
        }
    }
}
using System;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace MonoGameLibrary.Extensions.General.States {
    /// <summary>
    /// Module that integrates the state service with the GameHost lifecycle.
    /// </summary>
    public sealed class StateModule : IUpdateable, IDrawable {
        private readonly IStateService _service;
        private readonly IInputService _serviceInput;
        private readonly int _order;
        private bool _flagEnabled = true;
        private bool _flagVisible = true;
        
        public StateModule(IStateService service, IInputService serviceInput, int order = 0) {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (serviceInput == null) throw new ArgumentNullException(nameof(serviceInput));
            _service = service;
            _serviceInput = serviceInput;
            _order = order;
        }
        
        public int Order { get { return _order; } }
        
        public bool Enabled {
            get { return _flagEnabled; }
            set { _flagEnabled = value; }
        }
        
        public bool Visible {
            get { return _flagVisible; }
            set { _flagVisible = value; }
        }
        
        public void Update(FrameTime timeFrame) {
            if (!_flagEnabled) { return; }
            _service.Update(timeFrame, _serviceInput);
        }
        
        public void Draw(FrameTime timeFrame, IRenderContext contextRender) {
            if (!_flagVisible) { return; }
            _service.Draw(timeFrame, contextRender);
        }
    }
}
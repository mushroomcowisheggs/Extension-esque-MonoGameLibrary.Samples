using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Adapters.Gum;
using MonoGameLibrary.Extensions.General.Scenes;
using MonoGameLibrary.Extensions.General.States;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace DungeonSlime.States;

public class SplashState : State {
    // Dependencies
    private readonly IContentService _serviceContent;
    private readonly IInputService _serviceInput;
    private readonly ISceneService _serviceScene;
    private readonly IGumService _serviceGum;
    private readonly Rectangle _boundsScreen;
    private readonly ILogger _logger;
    private readonly Func<IContentService, Scene> _factoryNextScene;
    
    // Animation state
    private float _timeDisplay;
    private float _alphaFade;
    private float _timeFade;
    private bool _flagShouldExit = false;
    private bool _flagWaitingForRelease = false;
    private HashSet<Keys> _setKeysToWaitForReleaseHash = new HashSet<Keys>();
    private bool _flagContentLoaded = false;
    private const float FADE_DURATION = 0.5f;
    private const float FADE_ALPHA = 1.0f;
    private const float DISPLAY_TIME = 5.0f;
    
    private static readonly Keys[] TriggerKeys = new[] { Keys.Enter, Keys.Space, Keys.Escape };
    
    // Loaded content (set in LoadContent)
    private Texture2D _textureLogo;
    
    // Splash screen layout data (constants or computed in Enter)
    private Rectangle _rectangleIconSource;
    private Vector2 _originIcon;
    private Vector2 _positionIcon;
    private Rectangle _rectangleWordmarkSource;
    private Vector2 _originWordmark;
    private Vector2 _positionWordmark;
    private Vector2 _scale;

    public SplashState(
        IContentService serviceContent, 
        IInputService serviceInput, 
        ISceneService serviceScene, 
        IGumService serviceGum, 
        Rectangle boundsScreen, 
        ILogger logger, 
        Func<IContentService, Scene> factoryNextState, 
        Optional<IProfiler> profiler = default
    ) : base(serviceContent, new Optional<ILogger>(logger), profiler, new Optional<IInputService>(serviceInput)) {
        if (serviceContent == null) { throw new ArgumentNullException(nameof(serviceContent)); }
        if (serviceInput == null) { throw new ArgumentNullException(nameof(serviceInput)); }
        if (serviceScene == null) { throw new ArgumentNullException(nameof(serviceScene)); }
        if (logger == null) { throw new ArgumentNullException(nameof(logger)); }
        if (factoryNextState == null) { throw new ArgumentNullException(nameof(factoryNextState)); }
        _serviceContent = serviceContent;
        _serviceInput = serviceInput;
        _serviceScene = serviceScene;
        _serviceGum = serviceGum;
        _boundsScreen = boundsScreen;
        _logger = logger;
        _factoryNextScene = factoryNextState;
        
        _timeFade = FADE_DURATION;
        
        _rectangleIconSource = new Rectangle(0, 0, 128, 128);
        _originIcon = new Vector2(64, 64);
        _positionIcon = new Vector2(640, 200);
        // _rectangleWordmarkSource = new Rectangle(0, 128, 256, 64);
        _rectangleWordmarkSource = new Rectangle(150, 34, 458, 58);
        _originWordmark = new Vector2(128, 32);
        _positionWordmark = new Vector2(640, 400);
        _scale = new Vector2(1f, 1f);
        
        _flagContentLoaded = false;
    }
    
    /// <summary>
    /// Loads the splash screen content (texture).
    /// </summary>
    public override void LoadContent()
    {
        if (_flagContentLoaded) { return; }
        
        _textureLogo = _serviceContent.Load<Texture2D>("images/logo");
        
        if (_textureLogo == null) {
            this.Logger.Error("Failed to load splash screen logo texture.");
        }
        
        _flagContentLoaded = true;
    }
    
    public override void Enter() {
        if (!_flagContentLoaded) {
            LoadContent();
        }
        _timeDisplay = DISPLAY_TIME;
        _alphaFade = FADE_ALPHA;
        _flagShouldExit = false;
    }
    
    public override void HandleInput(FrameTime timeFrame, IInputService serviceInput) {
        if (_flagWaitingForRelease) { return; }
        
        var keysTrigger = new[] { Keys.Enter, Keys.Space, Keys.Escape };
        foreach (var key in keysTrigger) {
            if (serviceInput.WasKeyJustPressed(key)) {
                _setKeysToWaitForReleaseHash.Add(key);
                _flagWaitingForRelease = true;
                break;
            }
        }
    }
    
    public override void Update(FrameTime timeFrame) {
        if (_flagWaitingForRelease) {
            bool flagAllReleased = true;
            foreach (var key in _setKeysToWaitForReleaseHash) {
                if (_serviceInput.KeyboardState.IsKeyDown(key)) {
                    flagAllReleased = false;
                    break;
                }
            }
            if (flagAllReleased) {
                PerformExit();
                _flagWaitingForRelease = false;
                _setKeysToWaitForReleaseHash.Clear();
            }
            return;
        }
        
        float timeDelta = (float)timeFrame.DeltaTimeSpan.TotalSeconds;
        _timeDisplay -= timeDelta;

        if (_timeDisplay <= 0) {
            bool flagAnyKeyDown = false;
            foreach (var key in TriggerKeys) {
                if (_serviceInput.KeyboardState.IsKeyDown(key)) {
                    flagAnyKeyDown = true;
                    _setKeysToWaitForReleaseHash.Add(key);
                }
            }
            if (flagAnyKeyDown) {
                _flagWaitingForRelease = true;
            } else {
                PerformExit();
            }
            return;
        }
        
        if (_timeDisplay <= _timeFade && _timeDisplay > 0) {
            _alphaFade = _timeDisplay / _timeFade;
        }
    }
    
    private void PerformExit(){ 
        if (_flagShouldExit) { return; }
        _flagShouldExit = true;
        _serviceGum.ClearRoot();
        _serviceScene.ChangeScene(_factoryNextScene);
        RequestPop();
    }
    
    public override void Draw(FrameTime timeFrame, IRenderContext contextRender) {
        var batchSprite = contextRender.NativeContext as SpriteBatch;
        if (batchSprite == null) { return; }
        
        if (_textureLogo == null) {
            batchSprite.GraphicsDevice.Clear(Color.DarkSlateGray);
            return;
        }
        
        Vector2 center = new Vector2(_boundsScreen.Width / 2f, _boundsScreen.Height / 2f);
        Color tint = Color.White * _alphaFade;
        
        batchSprite.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        batchSprite.Begin(samplerState: SamplerState.PointClamp);
        
        // Draw the logo texture
        // Draw only the icon portion of the texture.
        batchSprite.Draw(
            _textureLogo, 
            _positionIcon, 
            _rectangleIconSource, 
            tint, 
            MathHelper.ToRadians(180), 
            _originIcon, 
            _scale, 
            SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 
            0
        );

        // Draw only the word mark portion of the texture.
        batchSprite.Draw(
            _textureLogo, 
            _positionWordmark, 
            _rectangleWordmarkSource, 
            tint, 
            MathHelper.ToRadians(180), 
            _originWordmark, 
            _scale, 
            SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 
            0
        );
        
        batchSprite.End();
    }
    
    protected override void Dispose(bool flagDisposing) {
        if (flagDisposing) {
            _textureLogo = null;
            _flagContentLoaded = false;
        }
        base.Dispose(flagDisposing);
    }
}
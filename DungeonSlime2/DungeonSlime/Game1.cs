using System;
using DungeonSlime.Scenes;
using DungeonSlime.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Adapters.MonoGame;
using MonoGameLibrary.Adapters.Gum;
using MonoGameLibrary.Extensions;
using MonoGameLibrary.Extensions.MonoGame.Audio;
using MonoGameLibrary.Extensions.MonoGame.Input;
using MonoGameLibrary.Extensions.General.Scenes;
using MonoGameLibrary.Extensions.General.States;
using MonoGameGum;
using MonoGameGum.Forms;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;

namespace DungeonSlime;

public class Game1 : Game {
    private readonly GraphicsDeviceManager _managerGraphicsDevice;
    private IGameHost _host;
    private MonoGameAdapter _adapter;
    private IContentService _serviceGlobalContent;
    private ILogger _logger;
    // The background theme song
    private Song _songTheme;
    private Rectangle _boundsScreen;
    
    public Game1(){
        _managerGraphicsDevice = new GraphicsDeviceManager(this) {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720,
            IsFullScreen = false
        };
        _managerGraphicsDevice.ApplyChanges();

        Window.Title = "Dungeon Slime";
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    protected override void Initialize() {
        // Reset the changed window size to 1280*720
        _managerGraphicsDevice.PreferredBackBufferWidth = 1280;
        _managerGraphicsDevice.PreferredBackBufferHeight = 720;
        _managerGraphicsDevice.ApplyChanges();
        
        // Build the game host: 
        var builder = new GameBuilder();
        var loggerConsole = new ConsoleLogger();
        var loggerFile = new FileLogger();
        _logger = loggerFile;
        builder.UseDefaultServices();
        builder.UseLogger(loggerFile, true);
        builder.UseAudio();
        builder.UseInput();
        builder.UseStates();
        
        _serviceGlobalContent = new MonoGameContentService(Content);
        builder.RegisterService<IContentService>(_serviceGlobalContent);
        
        var factoryContent = new MonoGameContentServiceFactory(Content.ServiceProvider, Content.RootDirectory);
        builder.RegisterService<IContentServiceFactory>(factoryContent);
        
        var logger = builder.GetService<ILogger>();
        var profiler = builder.TryGetService<IProfiler>(out var profilerInstance) 
        ? new Optional<IProfiler>(profilerInstance) 
        : default(Optional<IProfiler>);
        var serviceScene = new SceneService(
            _serviceGlobalContent, 
            factoryContent: factoryContent, 
            logger: new Optional<ILogger>(loggerFile), 
            profiler: profiler
        );
        builder.RegisterService<ISceneService>(serviceScene);
        builder.AddModule(new SceneModule(serviceScene));
        
        builder.UseGum(this, DefaultVisualsVersion.V3);
        
        // Register game controller service
        var serviceInput = builder.GetService<IInputService>();
        var controllerGame = new GameController(serviceInput);
        builder.RegisterService<IGameController>(controllerGame);
        
        _host = builder.Build();
        _host.OnError = delegate(Exception exception, string context) {
            System.Diagnostics.Trace.WriteLine($"Error in {context}: {exception}");
        };
        
        // Load adapter content: 
        var batchSprite = new SpriteBatch(GraphicsDevice);
        _adapter = new MonoGameAdapter(_host, batchSprite);
        
        _adapter.LoadContent(_serviceGlobalContent);
        
        base.Initialize();
        
        // Initialize gum: 
        _boundsScreen = GraphicsDevice.PresentationParameters.Bounds;
        
        // Initialize the Gum UI service
        var serviceGum = _host.Services.Get<IGumService>();
        
        // The assets created for the UI were done so at 1/4th the size to keep the size of the
        // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
        // the game's resolution then tell gum to zoom in by a factor of 4.
        float widthCanvas = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        float heightCanvas = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        serviceGum.SetCanvas(widthCanvas, heightCanvas, 4.0f);
        
        // Register input for UI control.
        serviceGum.ConfigureInput(flagEnableKeyboard: true, flagEnableGamepad: true);
        
        // Customize the tab reverse UI navigation to also trigger when the keyboard
        // Up arrow key is pushed. 
        serviceGum.AddTabReverseKey(Keys.Up);
        // Customize the tab UI navigation to also trigger when the keyboard
        // Down arrow key is pushed.
        serviceGum.AddTabForwardKey(Keys.Down);
        
        
        // Start title scene: 
        // Start the game with the splash state before setting the title scene.
        var serviceState = _host.Services.Get<IStateService>();
        // serviceScene = _host.Services.Get<ISceneService>();
        var serviceAudio = _host.Services.Get<IAudioService>();
        // serviceInput = _host.Services.Get<IInputService>();
        // logger = _host.Services.Get<ILogger>();
        // profiler = _host.Services.TryGet<IProfiler>(out var profilerInstance) ? new Optional<IProfiler>(profilerInstance) : default(Optional<IProfiler>);
        // controllerGame = _host.Services.Get<IGameController>();
        
        Func<IContentService, Scene> factoryTitleScene = delegate(IContentService serviceContent) {
            return new TitleScene(
                serviceContent,
                serviceAudio,
                serviceInput,
                serviceScene,
                serviceGum,
                Exit,
                _boundsScreen,
                controllerGame,
                new Optional<ILogger>(_logger),
                profiler
            );
        };
        
        var stateSplash = new SplashState(
            _serviceGlobalContent, 
            serviceInput, 
            serviceScene, 
            serviceGum, 
            _boundsScreen, 
            _logger, 
            factoryTitleScene, 
            profiler
        );
        serviceState.Push(stateSplash);
    }
    
    protected override void LoadContent() {
        // Load the background music
        _songTheme = _serviceGlobalContent.Load<Song>("audio/theme");
        var serviceAudio = _host.Services.Get<IAudioService>();
        serviceAudio.PlaySong(_songTheme, flagIsRepeating: true);
    }
    
    protected override void Update(GameTime timeGame) {
        _adapter.Update(timeGame);
        base.Update(timeGame);
    }
    
    protected override void Draw(GameTime timeGame) {
        // Do not execute Clear. Pass it to the scene.
        _adapter.Draw(timeGame);
        base.Draw(timeGame);
    }
}

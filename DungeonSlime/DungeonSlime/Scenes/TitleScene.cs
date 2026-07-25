using System;
using DungeonSlime.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Adapters.Gum;
using MonoGameLibrary.Adapters.MonoGame;
using MonoGameLibrary.Extensions.MonoGame.Audio;
using MonoGameLibrary.Extensions.MonoGame.Input;
using MonoGameLibrary.Extensions.MonoGame.Graphics;
using MonoGameLibrary.Extensions.General.Scenes;

namespace DungeonSlime.Scenes;

public class TitleScene : Scene {
    private const string DUNGEON_TEXT = "Dungeon";
    private const string SLIME_TEXT = "Slime";
    private const string PRESS_ENTER_TEXT = "Press Enter To Start";
    
    // Dependencies
    private readonly IContentService _serviceContent;
    private readonly IAudioService _serviceAudio;
    private readonly IInputService _serviceInput;
    private readonly ISceneService _serviceScene;
    private readonly IGumService _serviceGum;
    private readonly Action _actionExit;
    private readonly Rectangle _boundsScreen;
    private readonly IGameController _controllerGame;
    private readonly ILogger _logger;
    // private readonly IContentServiceFactory _factoryContent;
    
    // The font to use to render normal text.
    private SpriteFont _font;
    
    // The font used to render the title text.
    private SpriteFont _font5x;
    
    // The position to draw the dungeon text at.
    private Vector2 _positionDungeonText;
    
    // The origin to set for the dungeon text.
    private Vector2 _originDungeonText;
    
    // The position to draw the slime text at.
    private Vector2 _positionSlimeText;
    
    // The origin to set for the slime text.
    private Vector2 _originSlimeText;
    
    // The position to draw the press enter text at.
    private Vector2 _positionPressEnter;
    
    // The origin to set for the press enter text when drawing it.
    private Vector2 _originPressEnter;
    
    // The texture used for the background pattern.
    private Texture2D _patternBackground;
    
    // The destination rectangle for the background pattern to fill.
    private Rectangle _destinationBackground;
    
    // The offset to apply when drawing the background pattern so it appears to
    // be scrolling.
    private Vector2 _offsetBackground;
    
    // The speed that the background pattern scrolls.
    private float _speedScroll = 50.0f;
    
    private SoundEffect _effectUISound;
    private Panel _panelTitleScreenButtons;
    private Panel _panelOptions;
    
    // The options button used to open the options menu.
    private AnimatedButton _buttonOptions;
    
    // The back button used to exit the options menu back to the title menu.
    private AnimatedButton _buttonOptionsBack;
    
    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;
    
    public TitleScene(
        IContentService serviceContent, 
        IAudioService serviceAudio, 
        IInputService serviceInput, 
        ISceneService serviceScene, 
        IGumService serviceGum, 
        Action actionExit, 
        Rectangle boundsScreen, 
        IGameController controllerGame, 
        Optional<ILogger> logger = default, 
        Optional<IProfiler> profiler = default
    ) : base(serviceContent, logger) {
        if (serviceAudio == null) { throw new ArgumentNullException(nameof(serviceAudio)); }
        if (serviceInput == null) { throw new ArgumentNullException(nameof(serviceInput)); }
        if (serviceScene == null) { throw new ArgumentNullException(nameof(serviceScene)); }
        if (serviceGum == null) { throw new ArgumentNullException(nameof(serviceGum)); }
        if (actionExit == null) { throw new ArgumentNullException(nameof(actionExit)); }
        if (controllerGame == null) { throw new ArgumentNullException(nameof(controllerGame)); }
        _serviceContent = serviceContent;
        _serviceAudio = serviceAudio;
        _serviceInput = serviceInput;
        _serviceScene = serviceScene;
        _serviceGum = serviceGum;
        _actionExit = actionExit;
        _boundsScreen = boundsScreen;
        _controllerGame = controllerGame;
        _logger = logger.HasValue ? logger.Value : NullLogger.Instance;
    }
    
    // Initializes the UI components for this scene. 
    private void InitializeUI() {
        // Clear out any previous UI in case we came here from
        // a different screen:
        _serviceGum.ClearRoot();
        
        CreateTitlePanel();
        CreateOptionsPanel();
    }
    
    // Initializes the scene after content is loaded. 
    public override void Initialize() {
        // LoadContent is called during base.Initialize(). 
        base.Initialize();
        
        // Set the position and origin for the Dungeon text.
        Vector2 size = _font5x.MeasureString(DUNGEON_TEXT);
        _positionDungeonText = new Vector2(640, 100);
        _originDungeonText = size * 0.5f;
        
        // Set the position and origin for the Slime text.
        size = _font5x.MeasureString(SLIME_TEXT);
        _positionSlimeText = new Vector2(757, 207);
        _originSlimeText = size * 0.5f;
        
        // Set the position and origin for the press enter text.
        size = _font.MeasureString(PRESS_ENTER_TEXT);
        _positionPressEnter = new Vector2(640, 620);
        _originPressEnter = size * 0.5f;
        
        // Initialize the offset of the background pattern at zero
        _offsetBackground = Vector2.Zero;
        
        // Set the background pattern destination rectangle to fill the entire
        // screen background
        _destinationBackground = _boundsScreen;
        _offsetBackground = Vector2.Zero;
        
        InitializeUI();
    }
    
    // Loads all content for the scene. 
    public override void LoadContent() {
        // Load the font for the standard text.
        _font = _serviceContent.Load<SpriteFont>("fonts/04B_30");
        
        // Load the font for the title text
        _font5x = _serviceContent.Load<SpriteFont>("fonts/04B_30_5x");
        
        // Load the background pattern texture.
        _patternBackground = _serviceContent.Load<Texture2D>("images/background-pattern");
        
        // Load the sound effect to play when ui actions occur.
        _effectUISound = _serviceContent.Load<SoundEffect>("audio/ui");
        
        // Load the texture atlas from the xml configuration file.
        _atlas = _serviceContent.FromFile<TextureAtlas>("images/atlas-definition.xml");
    }

    private void CreateTitlePanel() {
        // Create a container to hold all of our buttons
        _panelTitleScreenButtons = new Panel();
        _panelTitleScreenButtons.Dock(Gum.Wireframe.Dock.Fill);
        _panelTitleScreenButtons.AddToRoot();

        AnimatedButton buttonStart = new AnimatedButton(_atlas);
        buttonStart.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        buttonStart.X = 50;
        buttonStart.Y = -12;
        buttonStart.Text = "Start";
        buttonStart.Click += HandleStartClicked;
        _panelTitleScreenButtons.AddChild(buttonStart);

        _buttonOptions = new AnimatedButton(_atlas);
        _buttonOptions.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _buttonOptions.X = -50;
        _buttonOptions.Y = -12;
        _buttonOptions.Text = "Options";
        _buttonOptions.Click += HandleOptionsClicked;
        _panelTitleScreenButtons.AddChild(_buttonOptions);

        buttonStart.IsFocused = true;
    }

    private void HandleStartClicked(object sender, EventArgs arguments) {
        // A UI interaction occurred, play the sound effect
        _serviceAudio.PlaySoundEffect(_effectUISound);

        // Change to the game scene to start the game.
        _serviceScene.ChangeScene(delegate(IContentService serviceContent) {
            return new GameScene(
                serviceContent,
                _serviceAudio, 
                _serviceInput, 
                _serviceScene, 
                _serviceGum, 
                _actionExit, 
                _boundsScreen, 
                _controllerGame, 
                Logger != null ? new Optional<ILogger>(Logger) : default(Optional<ILogger>), 
                Profiler
            );
        });
    }

    private void HandleOptionsClicked(object sender, EventArgs arguments) {
        // A UI interaction occurred, play the sound effect
        _serviceAudio.PlaySoundEffect(_effectUISound);

        // Set the title panel to be invisible.
        _panelTitleScreenButtons.IsVisible = false;

        // Set the options panel to be visible.
        _panelOptions.IsVisible = true;

        // Give the back button on the options panel focus.
        _buttonOptionsBack.IsFocused = true;
    }

    private void CreateOptionsPanel() {
        _panelOptions = new Panel();
        _panelOptions.Dock(Gum.Wireframe.Dock.Fill);
        _panelOptions.IsVisible = false;
        _panelOptions.AddToRoot();
        
        TextRuntime textOptions = new TextRuntime();
        textOptions.X = 10;
        textOptions.Y = 10;
        textOptions.Text = "OPTIONS";
        textOptions.UseCustomFont = true;
        textOptions.FontScale = 0.5f;
        textOptions.CustomFontFile = @"fonts/04b_30.fnt";
        _panelOptions.AddChild(textOptions);
        
        OptionsSlider sliderMusic = new OptionsSlider(_atlas);
        sliderMusic.Name = "MusicSlider";
        sliderMusic.Text = "MUSIC";
        sliderMusic.Anchor(Gum.Wireframe.Anchor.Top);
        sliderMusic.Y = 30f;
        sliderMusic.Minimum = 0;
        sliderMusic.Maximum = 1;
        sliderMusic.Value = _serviceAudio.SongVolume;
        sliderMusic.SmallChange = .1;
        sliderMusic.LargeChange = .2;
        sliderMusic.ValueChanged += HandleMusicSliderValueChanged;
        sliderMusic.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _panelOptions.AddChild(sliderMusic);

        OptionsSlider sfxSlider = new OptionsSlider(_atlas);
        sfxSlider.Name = "SfxSlider";
        sfxSlider.Text = "SFX";
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = _serviceAudio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _panelOptions.AddChild(sfxSlider);

        _buttonOptionsBack = new AnimatedButton(_atlas);
        _buttonOptionsBack.Text = "BACK";
        _buttonOptionsBack.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _buttonOptionsBack.X = -28f;
        _buttonOptionsBack.Y = -10f;
        _buttonOptionsBack.Click += HandleOptionsButtonBack;
        _panelOptions.AddChild(_buttonOptionsBack);
    }

    private void HandleSfxSliderChanged(object sender, EventArgs args) {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global sound effect volume to the value of the slider.;
        _serviceAudio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs arguments) {
        // Play the UI Sound effect so the player can hear the difference in audio.
        _serviceAudio.PlaySoundEffect(_effectUISound);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs args) {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global song volume to the value of the slider.
        _serviceAudio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args) {
        // A UI interaction occurred, play the sound effect
        _serviceAudio.PlaySoundEffect(_effectUISound);
    }

    private void HandleOptionsButtonBack(object sender, EventArgs arguments) {
        // A UI interaction occurred, play the sound effect
        _serviceAudio.PlaySoundEffect(_effectUISound);
        
        // Set the title panel to be visible.
        _panelTitleScreenButtons.IsVisible = true;
        
        // Set the options panel to be invisible.
        _panelOptions.IsVisible = false;
        
        // Give the options button on the title panel focus since we are coming
        // back from the options screen.
        _buttonOptions.IsFocused = true;
    }
    
    public override void Update(FrameTime timeFrame) {
        var timeGame = new GameTime(timeFrame.TotalTimeSpan, timeFrame.DeltaTimeSpan);
        
        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        if (_serviceInput.WasKeyJustPressed(Keys.Escape)) {
            _actionExit();
        }
        
        // Update the offsets for the background pattern wrapping so that it
        // scrolls down and to the right.
        float offset = _speedScroll * (float)timeFrame.DeltaTimeSpan.TotalSeconds;
        _offsetBackground.X -= offset;
        _offsetBackground.Y -= offset;

        // Ensure that the offsets do not go beyond the texture bounds so it is
        // a seamless wrap
        _offsetBackground.X %= _patternBackground.Width;
        _offsetBackground.Y %= _patternBackground.Height;
        
        _serviceGum.Update(timeGame);
    }

    public override void Draw(FrameTime timeFrame, IRenderContext context) {
        SpriteBatch batchSprite = context.NativeContext as SpriteBatch;
        if (batchSprite == null) { return; }
        
        batchSprite.GraphicsDevice.Clear(new Color(32, 40, 78, 255));
        
        // Draw the background pattern first using the PointWrap sampler state.
        batchSprite.Begin(samplerState: SamplerState.PointWrap);
        batchSprite.Draw(_patternBackground, _destinationBackground, new Rectangle(_offsetBackground.ToPoint(), _destinationBackground.Size), Color.White * 0.5f);
        batchSprite.End();
        
        if (_panelTitleScreenButtons.IsVisible) {
            // Begin the sprite batch to prepare for rendering.
            batchSprite.Begin(samplerState: SamplerState.PointClamp);
            
            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;
            
            // Draw the Dungeon text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            batchSprite.DrawString(_font5x, DUNGEON_TEXT, _positionDungeonText + new Vector2(10, 10), dropShadowColor, 0.0f, _originDungeonText, 1.0f, SpriteEffects.None, 1.0f);
            
            // Draw the Dungeon text on top of that at its original position
            batchSprite.DrawString(_font5x, DUNGEON_TEXT, _positionDungeonText, Color.White, 0.0f, _originDungeonText, 1.0f, SpriteEffects.None, 1.0f);
            
            // Draw the Slime text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            batchSprite.DrawString(_font5x, SLIME_TEXT, _positionSlimeText + new Vector2(10, 10), dropShadowColor, 0.0f, _originSlimeText, 1.0f, SpriteEffects.None, 1.0f);
            
            // Draw the Slime text on top of that at its original position
            batchSprite.DrawString(_font5x, SLIME_TEXT, _positionSlimeText, Color.White, 0.0f, _originSlimeText, 1.0f, SpriteEffects.None, 1.0f);
            
            // Always end the sprite batch when finished.
            batchSprite.End();
        }
        
        _serviceGum.Draw();
    }
}

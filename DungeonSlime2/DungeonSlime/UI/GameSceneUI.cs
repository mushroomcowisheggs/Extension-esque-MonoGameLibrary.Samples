using System;
using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Adapters.Gum;
using MonoGameLibrary.Extensions.MonoGame.Graphics;
using MonoGameLibrary.Extensions.MonoGame.Audio;

namespace DungeonSlime.UI;

public class GameSceneUI : ContainerRuntime {
    // The string format to use when updating the text for the score display.
    private static readonly string smv_formatScore = "SCORE: {0:D6}";
    
    // Dependencies
    private readonly IAudioService _serviceAudio;
    private readonly IContentService _serviceContent;
    private readonly IGumService _serviceGum;
    private readonly TextureAtlas _atlas;
    
    // The sound effect to play for auditory feedback of the user interface.
    private readonly SoundEffect _effectUISound;
    
    // The pause panel
    private Panel _panelPause;

    // The resume button on the pause panel. Field is used to track reference so
    // focus can be set when the pause panel is shown.
    private AnimatedButton _buttonResume;

    // The game over panel.
    private Panel _panelGameOver;

    // The retry button on the game over panel. Field is used to track reference
    // so focus can be set when the game over panel is shown.
    private AnimatedButton _buttonRetry;

    // The text runtime used to display the players score on the game screen.
    private TextRuntime _runtimeScoreText;

    /// <summary>
    /// Event invoked when the Resume button on the Pause panel is clicked.
    /// </summary>
    public event EventHandler ResumeButtonClick;

    /// <summary>
    /// Event invoked when the Quit button on either the Pause panel or the
    /// Game Over panel is clicked.
    /// </summary>
    public event EventHandler QuitButtonClick;

    /// <summary>
    /// Event invoked when the Retry button on the Game Over panel is clicked.
    /// </summary>
    public event EventHandler RetryButtonClick;

    public GameSceneUI(IAudioService serviceAudio, IContentService serviceContent, IGumService serviceGum, TextureAtlas atlas) {
        if (serviceAudio == null) { throw new ArgumentNullException(nameof(serviceAudio)); }
        if (serviceContent == null) { throw new ArgumentNullException(nameof(serviceContent)); }
        if (serviceGum == null) { throw new ArgumentNullException(nameof(serviceGum)); }
        if (atlas == null) { throw new ArgumentNullException(nameof(atlas)); }
        
        _serviceAudio = serviceAudio;
        _serviceContent = serviceContent;
        _serviceGum = serviceGum;
        _atlas = atlas;
        
        // Use that content service to load the sound effect for the user 
        // interface elements
        _effectUISound = _serviceContent.Load<SoundEffect>("audio/ui");
        
        // The game scene UI inherits from ContainerRuntime, so we set its
        // doc to fill so it fills the entire screen.
        Dock(Gum.Wireframe.Dock.Fill);

        // Add it to the root element.
        _serviceGum.AddToRoot(this);
        
        // Create the text that will display the players score and add it as
        // a child to this container.
        _runtimeScoreText = CreateScoreText();
        AddChild(_runtimeScoreText);
        
        // Create the Pause panel that is displayed when the game is paused and
        // add it as a child to this container
        _panelPause = CreatePausePanel(atlas);
        AddChild(_panelPause.Visual);
        
        // Create the Game Over panel that is displayed when a game over occurs
        // and add it as a child to this container
        _panelGameOver = CreateGameOverPanel(atlas);
        AddChild(_panelGameOver.Visual);
    }

    private TextRuntime CreateScoreText() {
        TextRuntime text = new TextRuntime();
        text.Anchor(Gum.Wireframe.Anchor.TopLeft);
        text.WidthUnits = DimensionUnitType.RelativeToChildren;
        text.X = 20.0f;
        text.Y = 5.0f;
        text.UseCustomFont = true;
        text.CustomFontFile = @"fonts/04b_30.fnt";
        text.FontScale = 0.25f;
        text.Text = string.Format(smv_formatScore, 0);
        
        return text;
    }

    private Panel CreatePausePanel(TextureAtlas atlas) {
        Panel panel = new Panel();
        panel.Anchor(Gum.Wireframe.Anchor.Center);
        panel.WidthUnits = DimensionUnitType.Absolute;
        panel.HeightUnits = DimensionUnitType.Absolute;
        panel.Width = 264.0f;
        panel.Height = 70.0f;
        panel.IsVisible = false;
        
        TextureRegion regionBackground = atlas.GetRegion("panel-background");
        
        NineSliceRuntime runtimeBackground = new NineSliceRuntime();
        runtimeBackground.Dock(Gum.Wireframe.Dock.Fill);
        runtimeBackground.Texture = regionBackground.Texture;
        runtimeBackground.TextureAddress = TextureAddress.Custom;
        runtimeBackground.TextureHeight = regionBackground.Height;
        runtimeBackground.TextureWidth = regionBackground.Width;
        runtimeBackground.TextureTop = regionBackground.SourceRectangle.Top;
        runtimeBackground.TextureLeft = regionBackground.SourceRectangle.Left;
        panel.AddChild(runtimeBackground);
        
        TextRuntime text = new TextRuntime();
        text.Text = "PAUSED";
        text.UseCustomFont = true;
        text.CustomFontFile = "fonts/04b_30.fnt";
        text.FontScale = 0.5f;
        text.X = 10.0f;
        text.Y = 10.0f;
        panel.AddChild(text);
        
        _buttonResume = new AnimatedButton(atlas);
        _buttonResume.Text = "RESUME";
        _buttonResume.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        _buttonResume.X = 9.0f;
        _buttonResume.Y = -9.0f;
        
        _buttonResume.Click += OnResumeButtonClicked;
        _buttonResume.GotFocus += OnElementGotFocus;
        
        panel.AddChild(_buttonResume);
        
        AnimatedButton buttonQuit = new AnimatedButton(atlas);
        buttonQuit.Text = "QUIT";
        buttonQuit.Anchor(Gum.Wireframe.Anchor.BottomRight);
        buttonQuit.X = -9.0f;
        buttonQuit.Y = -9.0f;
        
        buttonQuit.Click += OnQuitButtonClicked;
        buttonQuit.GotFocus += OnElementGotFocus;
        
        panel.AddChild(buttonQuit);
        
        return panel;
    }

    private Panel CreateGameOverPanel(TextureAtlas atlas) {
        Panel panel = new Panel();
        panel.Anchor(Gum.Wireframe.Anchor.Center);
        panel.WidthUnits = DimensionUnitType.Absolute;
        panel.HeightUnits = DimensionUnitType.Absolute;
        panel.Width = 264.0f;
        panel.Height = 70.0f;
        panel.IsVisible = false;
        
        TextureRegion regionBackground = atlas.GetRegion("panel-background");
        
        NineSliceRuntime runtimeBackground = new NineSliceRuntime();
        runtimeBackground.Dock(Gum.Wireframe.Dock.Fill);
        runtimeBackground.Texture = regionBackground.Texture;
        runtimeBackground.TextureAddress = TextureAddress.Custom;
        runtimeBackground.TextureHeight = regionBackground.Height;
        runtimeBackground.TextureWidth = regionBackground.Width;
        runtimeBackground.TextureTop = regionBackground.SourceRectangle.Top;
        runtimeBackground.TextureLeft = regionBackground.SourceRectangle.Left;
        panel.AddChild(runtimeBackground);
        
        TextRuntime text = new TextRuntime();
        text.Text = "GAME OVER";
        text.WidthUnits = DimensionUnitType.RelativeToChildren;
        text.UseCustomFont = true;
        text.CustomFontFile = "fonts/04b_30.fnt";
        text.FontScale = 0.5f;
        text.X = 10.0f;
        text.Y = 10.0f;
        panel.AddChild(text);
        
        _buttonRetry = new AnimatedButton(atlas);
        _buttonRetry.Text = "RETRY";
        _buttonRetry.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        _buttonRetry.X = 9.0f;
        _buttonRetry.Y = -9.0f;
        
        _buttonRetry.Click += OnRetryButtonClicked;
        _buttonRetry.GotFocus += OnElementGotFocus;
        
        panel.AddChild(_buttonRetry);
        
        AnimatedButton buttonQuit = new AnimatedButton(atlas);
        buttonQuit.Text = "QUIT";
        buttonQuit.Anchor(Gum.Wireframe.Anchor.BottomRight);
        buttonQuit.X = -9.0f;
        buttonQuit.Y = -9.0f;
        
        buttonQuit.Click += OnQuitButtonClicked;
        buttonQuit.GotFocus += OnElementGotFocus;
        
        panel.AddChild(buttonQuit);
        
        return panel;
    }
    
    private void OnResumeButtonClicked(object sender, EventArgs args) {
        // Button was clicked, play the ui sound effect for auditory feedback.
        _serviceAudio.PlaySoundEffect(_effectUISound);

        // Since the resume button was clicked, we need to hide the pause panel.
        HidePausePanel();

        // Invoke the ResumeButtonClick event
        if (ResumeButtonClick != null) {
            ResumeButtonClick(sender, args);
        }
    }
    
    private void OnRetryButtonClicked(object sender, EventArgs args) {
        // Button was clicked, play the ui sound effect for auditory feedback.
        _serviceAudio.PlaySoundEffect(_effectUISound);
        
        // Since the retry button was clicked, we need to hide the game over panel.
        HideGameOverPanel();
        
        // Invoke the RetryButtonClick event.
        if (RetryButtonClick != null) {
            RetryButtonClick(sender, args);
        }
    }

    private void OnQuitButtonClicked(object sender, EventArgs args) {
        // Button was clicked, play the ui sound effect for auditory feedback.
        _serviceAudio.PlaySoundEffect(_effectUISound);
        
        // Both panels have a quit button, so hide both panels
        HidePausePanel();
        HideGameOverPanel();
        
        // Invoke the QuitButtonClick event.
        if (QuitButtonClick != null) {
            QuitButtonClick(sender, args);
        }
    }
    
    private void OnElementGotFocus(object sender, EventArgs args) {
        // A ui element that can receive focus has received focus, play the
        // ui sound effect for auditory feedback.
        _serviceAudio.PlaySoundEffect(_effectUISound);
    }
    
    /// <summary>
    /// Updates the text on the score display.
    /// </summary>
    /// <param name="score">The score to display.</param>
    public void UpdateScoreText(int score) {
        _runtimeScoreText.Text = string.Format(smv_formatScore, score);
    }
    
    /// <summary>
    /// Tells the game scene ui to show the pause panel.
    /// </summary>
    public void ShowPausePanel() {
        _panelPause.IsVisible = true;
        
        // Give the resume button focus for keyboard/gamepad input.
        _buttonResume.IsFocused = true;
        
        // Ensure the game over panel isn't visible.
        _panelGameOver.IsVisible = false;
    }
    
    /// <summary>
    /// Tells the game scene ui to hide the pause panel.
    /// </summary>
    public void HidePausePanel() {
        _panelPause.IsVisible = false;
    }
    
    /// <summary>
    /// Tells the game scene ui to show the game over panel.
    /// </summary>
    public void ShowGameOverPanel() {
        _panelGameOver.IsVisible = true;
        
        // Give the retry button focus for keyboard/gamepad input.
        _buttonRetry.IsFocused = true;
        
        // Ensure the pause panel isn't visible.
        _panelPause.IsVisible = false;
    }
    
    /// <summary>
    /// Tells the game scene ui to hide the game over panel.
    /// </summary>
    public void HideGameOverPanel() {
        _panelGameOver.IsVisible = false;
    }
    
    /// <summary>
    /// Updates the game scene ui.
    /// </summary>
    /// <param name="timeGame">A snapshot of the timing values for the current update cycle.</param>
    public void Update(GameTime timeGame) {
        _serviceGum.Update(timeGame);
    }
    
    /// <summary>
    /// Draws the game scene ui.
    /// </summary>
    public void Draw() {
        _serviceGum.Draw();
    }
}

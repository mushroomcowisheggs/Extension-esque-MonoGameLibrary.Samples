using System;
using DungeonSlime.GameObjects;
using DungeonSlime.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameLibrary.Core;
using MonoGameLibrary.Core.Hosting;
using MonoGameLibrary.Core.Diagnostics;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Core.Lifecycle;
using MonoGameLibrary.Adapters.Gum;
using MonoGameLibrary.Adapters.MonoGame;
using MonoGameLibrary.Extensions.MonoGame.Graphics;
using MonoGameLibrary.Extensions.MonoGame.Audio;
using MonoGameLibrary.Extensions.MonoGame.Input;
using MonoGameLibrary.Extensions.General.Scenes;

namespace DungeonSlime.Scenes;

public class GameScene : Scene {
    private enum GameState {
        Playing,
        Paused,
        GameOver
    }
    
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
    
    // Reference to the slime.
    private Slime _slime;

    // Reference to the bat.
    private Bat _bat;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _boundsRoom;

    // The sound effect to play when the slime eats a bat.
    private SoundEffect _effectCollectSound;

    // The bounce sound effect for the bat
    private SoundEffect _effectBounceSound;
    
    // Tracks the players score.
    private int _score;

    private GameSceneUI _ui;

    private GameState _state;

    // The grayscale shader effect.
    private Effect _effectGrayscale;

    // The amount of saturation to provide the grayscale shader effect
    private float _saturation = 1.0f;

    // The speed of the fade to grayscale effect.
    private const float FADE_SPEED = 0.02f;

    // Reference to texture atlas (for UI buttons, etc.)
    private TextureAtlas _atlas;
    
    public GameScene(
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
    ) : base(serviceContent, logger, profiler) {
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
        // Clear out any previous UI element incase we came here
        // from a different scene.
        _serviceGum.ClearRoot();
        
        // Create the game scene ui instance.
        _ui = new GameSceneUI(_serviceAudio, ContentService, _serviceGum, _atlas);
        
        // Subscribe to the events from the game scene ui.
        _ui.ResumeButtonClick += OnResumeButtonClicked;
        _ui.RetryButtonClick += OnRetryButtonClicked;
        _ui.QuitButtonClick += OnQuitButtonClicked;
    }
    
    // Initializes the scene after content is loaded. 
    public override void Initialize() {
        // LoadContent is called during base.Initialize(). 
        base.Initialize();
        
        // Create the room bounds by getting the bounds of the screen then
        // using the Inflate method to "Deflate" the bounds by the width and
        // height of a tile so that the bounds only covers the inside room of
        // the dungeon tilemap.
        _boundsRoom = _boundsScreen;
        _boundsRoom.Inflate(-_tilemap.TileWidth, -_tilemap.TileHeight);
        
        // Create the animated sprite for the slime from the atlas.
        AnimatedSprite slimeAnimation = _atlas.CreateAnimatedSprite("slime-animation");
        slimeAnimation.Scale = new Vector2(4.0f, 4.0f);
        
        // Create the animated sprite for the bat from the atlas.
        AnimatedSprite batAnimation = _atlas.CreateAnimatedSprite("bat-animation");
        batAnimation.Scale = new Vector2(4.0f, 4.0f);
        
        
        // Create the game objects, injecting audio services.
        // Create the slime
        _slime = new Slime(slimeAnimation);
        // Create the bat
        _bat = new Bat(batAnimation, _effectBounceSound, _serviceAudio);
        
        // Subscribe to the slime's BodyCollision event so that a game over
        // can be triggered when this event is raised.
        _slime.BodyCollision += OnSlimeBodyCollision;
        
        // Initialize the user interface for the game scene.
        InitializeUI();
        
        // Initialize a new game to be played.
        InitializeNewGame();
    }
    
    // Loads all content for the scene. 
    public override void LoadContent() {
        // Use that content service to load the atlas for the user interface 
        // elements
        _atlas = _serviceContent.FromFile<TextureAtlas>("images/atlas-definition.xml");
        
        // Create the tilemap from the XML configuration file.
        _tilemap = _serviceContent.FromFile<Tilemap>("images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);
        
        // Load the bounce sound effect for the bat
        _effectBounceSound = _serviceContent.Load<SoundEffect>("audio/bounce");
        
        // Load the collect sound effect
        _effectCollectSound = _serviceContent.Load<SoundEffect>("audio/collect");
        
        // Load the grayscale effect
        _effectGrayscale = _serviceContent.Load<Effect>("effects/grayscaleEffect");
    }
    
    private void OnResumeButtonClicked(object sender, EventArgs args) {
        // Change the game state back to playing
        _state = GameState.Playing;
    }

    private void OnRetryButtonClicked(object sender, EventArgs args) {
        _serviceAudio.PlaySoundEffect(_effectCollectSound);
        
        // Switch to a fresh GameScene with a new content service
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

    private void OnQuitButtonClicked(object sender, EventArgs args) {
        // Player has chosen to quit, so return back to the title scene
        _serviceScene.ChangeScene(delegate(IContentService serviceContent) {
            return new TitleScene(
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
    
    private void InitializeNewGame() {
        // Calculate the position for the slime, which will be at the center
        // tile of the tile map.
        Vector2 positionSlime = new Vector2();
        positionSlime.X = (_tilemap.Columns / 2) * _tilemap.TileWidth;
        positionSlime.Y = (_tilemap.Rows / 2) * _tilemap.TileHeight;
        
        float strideOriginal = _tilemap.TileWidth;
        float stride = _tilemap.ScaledTileWidth;
        // Initialize the slime
        _slime.Initialize(positionSlime, stride);
        
        // Initialize the bat
        _bat.RandomizeVelocity();
        PositionBatAwayFromSlime();
        
        // Reset the score
        _score = 0;
        
        // Set the game state to playing
        _state = GameState.Playing;
        
        // Reset UI score display.
        _ui.UpdateScoreText(_score);
        _ui.HidePausePanel();
        _ui.HideGameOverPanel();
        
        // Reset saturation for grayscale.
        _saturation = 1.0f;
    }
    
    public override void Update(FrameTime timeFrame) {
        var timeGame = new GameTime(timeFrame.TotalTimeSpan, timeFrame.DeltaTimeSpan);
        _serviceGum.Update(timeGame);
        
        // Handle grayscale fade if not playing. 
        if (_state != GameState.Playing) {
            // The game is in either a paused or game over state, so
            // gradually decrease the saturation to create the fading grayscale. 
            _saturation = Math.Max(0.0f, _saturation - FADE_SPEED);
            
            // If its just a game over state, return back
            if (_state == GameState.GameOver) {
                return;
            }
        }

        // If the pause button is pressed, toggle the pause state
        if (_controllerGame.Pause()) {
            TogglePause();
        }

        // At this point, if the game is paused, just return back early
        if (_state == GameState.Paused) {
            return;
        }

        Vector2 directionPotentialNext = _controllerGame.GetDirection();
        
        _slime.SetDirection(directionPotentialNext);
        
        // Update game objects.
        // Update the slime;
        _slime.Update(timeFrame);
        // Update the bat;
        _bat.Update(timeFrame);

        // Perform collision checks
        CollisionChecks();
        
        // Ensure the UI is always updated
        _ui.Update(timeGame);
    }

    private void CollisionChecks() {
        // Capture the current bounds of the slime and bat
        Circle boundsSlime = _slime.GetBounds();
        Circle boundsBat = _bat.GetBounds();

        // FIrst perform a collision check to see if the slime is colliding with
        // the bat, which means the slime eats the bat.
        if (boundsSlime.Intersects(boundsBat)) {
            // Move the bat to a new position away from the slime.
            PositionBatAwayFromSlime();
            
            // Randomize the velocity of the bat.
            _bat.RandomizeVelocity();
            
            // Tell the slime to grow.
            _slime.Grow();
            
            // Increment the score.
            _score += 100;
            
            // Update the score display on the UI.
            _ui.UpdateScoreText(_score);
            
            // Play the collect sound effect
            _serviceAudio.PlaySoundEffect(_effectCollectSound);
        }

        // Next check if the slime is colliding with the wall by validating if
        // it is within the bounds of the room.  If it is outside the room
        // bounds, then it collided with a wall which triggers a game over.
        if (boundsSlime.Top < _boundsRoom.Top ||
           boundsSlime.Bottom > _boundsRoom.Bottom ||
           boundsSlime.Left < _boundsRoom.Left ||
           boundsSlime.Right > _boundsRoom.Right
        ) {
            GameOver();
            return;
        }

        // Finally, check if the bat is colliding with a wall by validating if
        // it is within the bounds of the room.  If it is outside the room
        // bounds, then it collided with a wall, and the bat should bounce
        // off of that wall.
        if (boundsBat.Top < _boundsRoom.Top) {
            _bat.Bounce(Vector2.UnitY);
        } else if (boundsBat.Bottom > _boundsRoom.Bottom) {
            _bat.Bounce(-Vector2.UnitY);
        }

        if (boundsBat.Left < _boundsRoom.Left) {
            _bat.Bounce(Vector2.UnitX);
        } else if (boundsBat.Right > _boundsRoom.Right) {
            _bat.Bounce(-Vector2.UnitX);
        }
    }
    
    private void PositionBatAwayFromSlime() {
        // Calculate the position that is in the center of the bounds
        // of the room.
        float positionRoomCenterX = _boundsRoom.X + _boundsRoom.Width * 0.5f;
        float positionRoomCenterY = _boundsRoom.Y + _boundsRoom.Height * 0.5f;
        Vector2 positionRoomCenter = new Vector2(positionRoomCenterX, positionRoomCenterY);
        
        // Get the bounds of the slime and calculate the center position
        Circle boundsSlime = _slime.GetBounds();
        Vector2 positionSlimeCenter = new Vector2(boundsSlime.X, boundsSlime.Y);
        
        // Calculate the distance vector from the center of the room to the
        // center of the slime.
        Vector2 vectorToSlimeCenter = positionSlimeCenter - positionRoomCenter;
        
        // Get the bounds of the bat
        Circle boundsBat = _bat.GetBounds();
        
        // Calculate the amount of padding we will add to the new position of
        // the bat to ensure it is not sticking to walls
        int amountPadding = boundsBat.Radius * 2;
        
        // Calculate the new position of the bat by finding which component of
        // the center to slime vector (X or Y) is larger and in which direction.
        Vector2 positionNewBat = Vector2.Zero;
        if (Math.Abs(vectorToSlimeCenter.X) > Math.Abs(vectorToSlimeCenter.Y)) {
            // The slime is closer to either the left or right wall, so the Y
            // position will be a random position between the top and bottom
            // walls.
            positionNewBat.Y = Random.Shared.Next(
                _boundsRoom.Top + amountPadding,
                _boundsRoom.Bottom - amountPadding
            );

            if (vectorToSlimeCenter.X > 0) {
                // The slime is closer to the right side wall, so place the
                // bat on the left side wall
                positionNewBat.X = _boundsRoom.Left + amountPadding;
            } else {
                // The slime is closer ot the left side wall, so place the
                // bat on the right side wall.
                positionNewBat.X = _boundsRoom.Right - amountPadding * 2;
            }
        } else {
            // The slime is closer to either the top or bottom wall, so the X
            // position will be a random position between the left and right
            // walls.
            positionNewBat.X = Random.Shared.Next(
                _boundsRoom.Left + amountPadding,
                _boundsRoom.Right - amountPadding
            );
            
            if (vectorToSlimeCenter.Y > 0) {
                // The slime is closer to the top wall, so place the bat on the
                // bottom wall
                positionNewBat.Y = _boundsRoom.Top + amountPadding;
            } else {
                // The slime is closer to the bottom wall, so place the bat on
                // the top wall.
                positionNewBat.Y = _boundsRoom.Bottom - amountPadding * 2;
            }
        }

        // Assign the new bat position
        _bat.Position = positionNewBat;
    }

    private void OnSlimeBodyCollision(object sender, EventArgs args) {
        GameOver();
    }

    private void TogglePause() {
        if (_state == GameState.Paused) {
            // We're now unpausing the game, so hide the pause panel
            _ui.HidePausePanel();
            
            // And set the state back to playing
            _state = GameState.Playing;
        } else {
            // We're now pausing the game, so show the pause panel
            _ui.ShowPausePanel();

            // And set the state to paused
            _state = GameState.Paused;

            // Set the grayscale effect saturation to 1.0f;
            _saturation = 1.0f;
        }
    }

    private void GameOver() {
        // Show the game over panel
        _ui.ShowGameOverPanel();

        // Set the game state to game over
        _state = GameState.GameOver;

        // Set the grayscale effect saturation to 1.0f;
        _saturation = 1.0f;
    }

    public override void Draw(FrameTime timeFrame, IRenderContext contextRender) {
        SpriteBatch batchSprite = contextRender.NativeContext as SpriteBatch;
        if (batchSprite == null) {
            throw new InvalidOperationException("RenderContext does not contain a SpriteBatch.");
        }
        
        // Clear the back buffer. 
        batchSprite.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // Begin sprite batch with optional effect. 
        if (_state != GameState.Playing) {
            // We are in a game over state, so apply the saturation parameter.
            _effectGrayscale.Parameters["Saturation"].SetValue(_saturation);
            
            // And begin the sprite batch using the grayscale effect.
            batchSprite.Begin(samplerState: SamplerState.PointClamp, effect: _effectGrayscale);
        } else {
            // Otherwise, just begin the sprite batch as normal.
            batchSprite.Begin(samplerState: SamplerState.PointClamp);
        }
        
        // Draw the tilemap
        _tilemap.Draw(batchSprite);
        
        // Draw the slime.
        _slime.Draw(batchSprite);
        
        // Draw the bat.
        _bat.Draw(batchSprite);
        
        // Always end the sprite batch when finished.
        batchSprite.End();
        
        // Draw the UI
        _ui.Draw();
    }
    
    protected override void Dispose(bool flagDisposing) {
        if (flagDisposing) {
            // Unsubscribe events to avoid leaks.
            if (_slime != null) {
                _slime.BodyCollision -= OnSlimeBodyCollision;
            }
        }
        
        base.Dispose(flagDisposing);
    }
}

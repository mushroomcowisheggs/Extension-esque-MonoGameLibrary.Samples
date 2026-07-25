using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Extensions.MonoGame.Input;

namespace DungeonSlime;

/// <summary>
/// Provides a game-specific input abstraction that maps physical inputs
/// to game actions, bridging our input system with game-specific functionality.
/// </summary>
public class GameController : IGameController {
    private readonly IInputService _serviceInput;
    
    public GameController(IInputService serviceInput) {
        if (serviceInput == null) {
            throw new ArgumentNullException(nameof(serviceInput));
        }
        _serviceInput = serviceInput;
    }
    
    /// <inheritdoc />
    public Vector2 GetDirection() {
        if (_serviceInput.WasKeyJustPressed(Keys.Up) || _serviceInput.WasKeyJustPressed(Keys.W) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.DPadUp) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.LeftThumbstickUp)
        ) {
            return -Vector2.UnitY;
        }
        if (_serviceInput.WasKeyJustPressed(Keys.Down) || _serviceInput.WasKeyJustPressed(Keys.S) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.DPadDown) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.LeftThumbstickDown)
        ) {
            return Vector2.UnitY;
        }
        if (_serviceInput.WasKeyJustPressed(Keys.Left) || _serviceInput.WasKeyJustPressed(Keys.A) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.DPadLeft) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.LeftThumbstickLeft)
        ) {
            return -Vector2.UnitX;
        }
        if (_serviceInput.WasKeyJustPressed(Keys.Right) || _serviceInput.WasKeyJustPressed(Keys.D) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.DPadRight) ||
            _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.LeftThumbstickRight)
        ) {
            return Vector2.UnitX;
        }
        return Vector2.Zero;
    }
    
    /// <inheritdoc />
    public bool Pause() {
        return _serviceInput.WasKeyJustPressed(Keys.Escape) ||
        _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.Start);
    }

    /// <inheritdoc />
    public bool Action()
    {
        return _serviceInput.WasKeyJustPressed(Keys.Enter) ||
        _serviceInput.WasGamePadButtonJustPressed(PlayerIndex.One, Buttons.A);
    }
}

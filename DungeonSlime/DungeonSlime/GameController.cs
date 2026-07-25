using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Extensions.Input;

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
        if (_serviceInput.WasKeyJustPressed(KeyCode.Up) || _serviceInput.WasKeyJustPressed(KeyCode.W) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadUp) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadUp)
        ) {
            return -Vector2.UnitY;
        }
        if (_serviceInput.WasKeyJustPressed(KeyCode.Down) || _serviceInput.WasKeyJustPressed(KeyCode.S) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadDown) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadDown)
        ) {
            return Vector2.UnitY;
        }
        if (_serviceInput.WasKeyJustPressed(KeyCode.Left) || _serviceInput.WasKeyJustPressed(KeyCode.A) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadLeft) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadLeft)
        ) {
            return -Vector2.UnitX;
        }
        if (_serviceInput.WasKeyJustPressed(KeyCode.Right) || _serviceInput.WasKeyJustPressed(KeyCode.D) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadRight) ||
            _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.DPadRight)
        ) {
            return Vector2.UnitX;
        }
        return Vector2.Zero;
    }
    
    /// <inheritdoc />
    public bool Pause() {
        return _serviceInput.WasKeyJustPressed(KeyCode.Escape) ||
        _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.Start);
    }

    /// <inheritdoc />
    public bool Action()
    {
        return _serviceInput.WasKeyJustPressed(KeyCode.Enter) ||
        _serviceInput.WasButtonJustPressed(MonoGameLibrary.Extensions.Input.PlayerIndex.One, MonoGameLibrary.Extensions.Input.GamePadButton.A);
    }
}

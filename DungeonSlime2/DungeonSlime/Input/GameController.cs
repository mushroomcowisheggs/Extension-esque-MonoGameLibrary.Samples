using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Extensions.Input;

namespace DungeonSlime.Input;

/// <summary>
/// Provides a game-specific input abstraction that maps physical inputs
/// to game actions, bridging our input system with game-specific functionality.
/// </summary>
public class GameController : IGameController {
    private readonly IInputMappingService _serviceInputMapping;
    
    public GameController(IInputMappingService serviceInputMapping) {
        if (serviceInputMapping == null) {
            throw new ArgumentNullException(nameof(serviceInputMapping));
        }
        _serviceInputMapping = serviceInputMapping;
    }
    
    /// <inheritdoc />
    public Vector2 GetDirection() {
        return _serviceInputMapping.GetActionDirection(
            GameAction.MoveUp,
            GameAction.MoveDown,
            GameAction.MoveLeft,
            GameAction.MoveRight
        );
    }
    
    /// <inheritdoc />
    public bool Pause() {
        return _serviceInputMapping.IsActionPressed(GameAction.Pause);
    }
    
    /// <inheritdoc />
    public bool Action() {
        return _serviceInputMapping.IsActionPressed(GameAction.Confirm);
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Graphics;
using MonoGameLibrary.Extensions.MonoGame.Audio;

namespace DungeonSlime.GameObjects;

public class Bat {
    private const float MOVEMENT_SPEED = 5.0f;
    
    // The velocity of the bat that defines the direction and how much in that
    // direction to update the bats position each update cycle.
    private Vector2 _velocity;
    
    // The animated sprite used when drawing the bat.
    private readonly AnimatedSprite _spriteAnimated;
    
    // The sound effect to play when the bat bounces off the edge of the room.
    private readonly SoundEffect _effectBounceSound;
    
    // Audio service for playing sounds.
    private readonly IAudioService _serviceAudio;
    
    /// <summary>
    /// Gets or Sets the position of the bat.
    /// </summary>
    public Vector2 Position { get; set; }
    
    /// <summary>
    /// Creates a new Bat using the specified animated sprite and sound effect.
    /// </summary>
    /// <param name="spriteAnimated">The animated sprite ot use when drawing the bat. </param>
    /// <param name="effectBounceSound">The sound effect to play when the bat bounces off a wall. </param>
    /// <param name="serviceAudio">The audio service for sound playback. </param>
    public Bat(AnimatedSprite spriteAnimated, SoundEffect effectBounceSound, IAudioService serviceAudio) {
        if (spriteAnimated == null) {
            throw new ArgumentNullException(nameof(spriteAnimated));
        }
        if (effectBounceSound == null) {
            throw new ArgumentNullException(nameof(effectBounceSound));
        }
        if (serviceAudio == null) {
            throw new ArgumentNullException(nameof(serviceAudio));
        }
        
        _spriteAnimated = spriteAnimated;
        _effectBounceSound = effectBounceSound;
        _serviceAudio = serviceAudio;
        _velocity = Vector2.Zero;
        Position = Vector2.Zero;
    }
    
    /// <summary>
    /// Randomizes the velocity of the bat.
    /// </summary>
    public void RandomizeVelocity() {
        // Generate a random angle
        float angle = (float)(Random.Shared.NextDouble() * MathHelper.TwoPi);
        
        // Convert the angle to a direction vector
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);
        
        // Multiply the direction vector by the movement speed to get the
        // final velocity
        _velocity = direction * MOVEMENT_SPEED;
    }
    
    /// <summary>
    /// Handles a bounce event when the bat collides with a wall or boundary.
    /// </summary>
    /// <param name="normal">The normal vector of the surface the bat is bouncing against.</param>
    public void Bounce(Vector2 normal) {
        Vector2 positionNew = Position;
        
        // Adjust the position based on the normal to prevent sticking to walls.
        if (normal.X != 0) {
            // We are bouncing off a vertical wall (left/right).
            // Move slightly away from the wall in the direction of the normal.
            positionNew.X += normal.X * (_spriteAnimated.Width * 0.1f);
        }
        
        if (normal.Y != 0) {
            // We are bouncing off a horizontal wall (top/bottom).
            // Move slightly way from the wall in the direction of the normal.
            positionNew.Y += normal.Y * (_spriteAnimated.Height * 0.1f);
        }
        
        // Apply the new position
        Position = positionNew;
        
        // Normalize before reflecting
        normal.Normalize();        
        
        // Apply reflection based on the normal.
        _velocity = Vector2.Reflect(_velocity, normal);
        
        // Play the bounce sound effect.
        _serviceAudio.PlaySoundEffect(_effectBounceSound);
    }
    
    /// <summary>
    /// Returns a Circle value that represents collision bounds of the bat.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds() {
        int x = (int)(Position.X + _spriteAnimated.Width * 0.5f);
        int y = (int)(Position.Y + _spriteAnimated.Height * 0.5f);
        int radius = (int)(_spriteAnimated.Width * 0.25f);
        
        return new Circle(x, y, radius);
    }
    
    /// <summary>
    /// Updates the bat.
    /// </summary>
    /// <param name="timeFrame">A snapshot of the timing values for the current update cycle.</param>
    public void Update(FrameTime timeFrame) {
        // Update the animated sprite
        _spriteAnimated.Update(timeFrame);
        
        // Update the position of the bat based on the velocity.
        Position += _velocity;
    }
    
    /// <summary>
    /// Draws the bat.
    /// </summary>
    public void Draw(SpriteBatch batchSprite) {
        if (batchSprite == null) {
            throw new ArgumentNullException(nameof(batchSprite));
        }
        
        _spriteAnimated.Draw(batchSprite, Position);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Core.Time;
using MonoGameLibrary.Extensions.MonoGame.Graphics;

namespace DungeonSlime.GameObjects;

public class Slime {
    // A constant value that represents the amount of time to wait between
    // movement updates.
    private static readonly TimeSpan somv_timeMovementInterval = TimeSpan.FromMilliseconds(200);
    
    // The amount of time that has elapsed since the last movement update.
    private TimeSpan _spanMovementTime;
    
    // Normalized value (0-1) representing progress between movement ticks for visual interpolation
    private float _progressMovement;
    
    // The next direction to apply to the head of the slime chain during the
    // next movement update.
    private Vector2 _directionNext;
    
    // The number of pixels to move the head segment during the movement cycle.
    private float _stride;
    
    // Tracks the segments of the slime chain.
    private readonly List<SlimeSegment> _listSegment;
    
    // The animated sprite used when drawing each slime segment
    private readonly AnimatedSprite _spriteAnimated;
    
    // Buffer to queue inputs input by player during input polling.
    private readonly Queue<Vector2> _queueInput;
    
    // The capacity ( == maximum size) of the buffer queue.
    private readonly int _capacityInputQueue;
    
    // The maximum size of the buffer queue.
    private const int MAX_BUFFER_SIZE = 2;
    
    /// <summary>
    /// Event that is raised if it is detected that the head segment of the slime
    /// has collided with a body segment.
    /// </summary>
    public event EventHandler BodyCollision;
    
    /// <summary>
    /// Creates a new Slime using the specified animated sprite. 
    /// </summary>
    /// <param name="spriteAnimated">The animated sprite to use when drawing the slime. </param>
    public Slime(AnimatedSprite spriteAnimated) {
        if (spriteAnimated == null) {
            throw new ArgumentNullException(nameof(spriteAnimated));
        }
        _spriteAnimated = spriteAnimated;
        _listSegment = new List<SlimeSegment>();
        // initialize the input buffer.
        _queueInput = new Queue<Vector2>(MAX_BUFFER_SIZE);
        _capacityInputQueue = MAX_BUFFER_SIZE;
        _spanMovementTime = TimeSpan.Zero;
        _progressMovement = 0.0f;
        _directionNext = Vector2.Zero;
        _stride = 0.0f;
    }
    
    /// <summary>
    /// Initializes the slime, can be used to reset it back to an initial state.
    /// </summary>
    /// <param name="positionStarting">The position the slime should start at.</param>
    /// <param name="stride">The total number of pixels to move the head segment during each movement cycle.</param>
    public void Initialize(Vector2 positionStarting, float stride) {
        // Initialize the segment collection.
        _listSegment.Clear();
        
        _queueInput.Clear();
        
        // Set the stride
        _stride = stride;
        
        // Create the initial head of the slime chain.
        SlimeSegment segmentHead = new SlimeSegment();
        segmentHead.At = positionStarting;
        segmentHead.To = positionStarting + new Vector2(_stride, 0);
        segmentHead.Direction = Vector2.UnitX;
        
        // Add it to the segment collection.
        _listSegment.Add(segmentHead);
        
        // Set the initial next direction as the same direction the head is
        // moving.
        _directionNext = segmentHead.Direction;
        
        // Zero out the movement timer.
        _spanMovementTime = TimeSpan.Zero;
        
        _progressMovement = 0.0f;
    }
    
    public void SetDirection(Vector2 direction) {
        if (direction == Vector2.Zero) {
            return;
        }
        if (_queueInput.Count >= _capacityInputQueue) {
            return;
        }
        
        // If the buffer is empty, validate against the current direction;
        // otherwise, validate against the last buffered direction
        Vector2 directionValidateAgainst = _queueInput.Count > 0 ? _queueInput.Last() : _listSegment[0].Direction;

        // Only allow direction change if it is not reversing the current
        // direction.  This prevents th slime from backing into itself
        float dot = Vector2.Dot(direction, directionValidateAgainst);
        if (dot >= 0) {
            _queueInput.Enqueue(direction);
        }
    }
    
    private void Move() {
        // Get the next direction from the input buffer if one is available
        if (_queueInput.Count > 0) {
            _directionNext = _queueInput.Dequeue();
        }
        
        // Capture the value of the head segment
        SlimeSegment segmentHead = _listSegment[0];
        
        // Update the direction the head is supposed to move in to the
        // next direction cached.
        segmentHead.Direction = _directionNext;
        
        // Update the head's "at" position to be where it was moving "to"
        segmentHead.At = segmentHead.To;
        
        // Update the head's "to" position to the next tile in the direction
        // it is moving.
        segmentHead.To = segmentHead.At + segmentHead.Direction * _stride;
        
        // Insert the new adjusted value for the head at the front of the
        // segments and remove the tail segment. This effectively moves
        // the entire chain forward without needing to loop through every
        // segment and update its "at" and "to" positions.
        _listSegment.Insert(0, segmentHead);
        _listSegment.RemoveAt(_listSegment.Count - 1);
        
        // Iterate through all of the segments except the head and check
        // if they are at the same position as the head. If they are, then
        // the head is colliding with a body segment and a body collision
        // has occurred.
        for (int i = 1; i < _listSegment.Count; i += 1) {
            SlimeSegment segment = _listSegment[i];
            
            if (segmentHead.At == segment.At) {
                if (BodyCollision != null) {
                    BodyCollision.Invoke(this, EventArgs.Empty);
                }
                
                return;
            }
        }
    }
    
    /// <summary>
    /// Informs the slime to grow by one segment.
    /// </summary>
    public void Grow() {
        // Capture the value of the tail segment
        SlimeSegment segmentTail = _listSegment[_listSegment.Count - 1];
        
        // Create a new tail segment that is positioned a grid cell in the
        // reverse direction from the tail moving to the tail.
        SlimeSegment newTail = new SlimeSegment();
        newTail.At = segmentTail.To + segmentTail.ReverseDirection * _stride;
        newTail.To = segmentTail.At;
        newTail.Direction = Vector2.Normalize(segmentTail.At - newTail.At);
        
        // Add the new tail segment
        _listSegment.Add(newTail);
    }
    
    /// <summary>
    /// Updates the slime.
    /// </summary>
    /// <param name="timeFrame">A snapshot of the timing values for the current update cycle.</param>
    public void Update(FrameTime timeFrame) {
        // Update the animated sprite.
        _spriteAnimated.Update(timeFrame);
        
        // Increment the movement timer by the frame elapsed time.
        _spanMovementTime += timeFrame.DeltaTimeSpan;
        
        // If the movement timer has accumulated enough time to be greater than
        // the movement time threshold, then perform a full movement.
        if (_spanMovementTime >= somv_timeMovementInterval) {
            _spanMovementTime -= somv_timeMovementInterval;
            Move();
        }
        
        // Update the movement lerp offset amount
        _progressMovement = (float)(_spanMovementTime.TotalSeconds / somv_timeMovementInterval.TotalSeconds);
    }
    
    /// <summary>
    /// Draws the slime.
    /// </summary>
    public void Draw(SpriteBatch batchSprite) {
        if (batchSprite == null) {
            throw new ArgumentNullException(nameof(batchSprite));
        }
        
        // Iterate through each segment and draw it
        foreach (SlimeSegment segment in _listSegment) {
            // Calculate the visual position of the segment at the moment by
            // lerping between its "at" and "to" position by the movement
            // offset lerp amount
            Vector2 position = Vector2.Lerp(segment.At, segment.To, _progressMovement);
            
            // Draw the slime sprite at the calculated visual position of this
            // segment
            _spriteAnimated.Draw(batchSprite, position);
        }
    }
    
    /// <summary>
    /// Returns a Circle value that represents collision bounds of the slime.
    /// </summary>
    /// <returns>A Circle value.</returns>
    public Circle GetBounds() {
        SlimeSegment segmentHead = _listSegment[0];
        
        // Calculate the visual position of the head at the moment of this
        // method call by lerping between the "at" and "to" position by the
        // movement offset lerp amount
        Vector2 position = Vector2.Lerp(segmentHead.At, segmentHead.To, _progressMovement);
        
        // Create the bounds using the calculated visual position of the head.
        Circle bounds = new Circle(
            (int)(position.X + (_spriteAnimated.Width * 0.5f)),
            (int)(position.Y + (_spriteAnimated.Height * 0.5f)),
            (int)(_spriteAnimated.Width * 0.5f)
        );
        
        return bounds;
    }
}

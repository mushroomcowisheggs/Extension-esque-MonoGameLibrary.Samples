using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Extensions.MonoGame.Graphics {
    /// <summary>
    /// A simple circle value type used by game code. 
    /// </summary>
    public readonly struct Circle {
        /// <summary>
        /// Gets the center x-coordinate.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the center y-coordinate. 
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the radius. 
        /// </summary>
        public int Radius { get; }

        /// <summary>
        /// Creates a new circle. 
        /// </summary>
        public Circle(int x, int y, int radius) {
            X = x;
            Y = y;
            Radius = radius;
        }

        /// <summary>
        /// Gets the location of the circle center. 
        /// </summary>
        public Point Location { get { return new Point(X, Y); } }
        
        public int Top { get { return Y - Radius; } }
        public int Bottom { get { return Y + Radius; } }
        public int Left { get { return X - Radius; } }
        public int Right { get { return X + Radius; } }
        
        public bool Equals(Circle other) { return X == other.X && Y == other.Y && Radius == other.Radius; }
        
        public bool Intersects(Circle other) {
            return Intersects(this, other);
        }
        
        public override bool Equals(object obj) { return obj is Circle other && Equals(other); }
        public override int GetHashCode() { return HashCode.Combine(X, Y, Radius); }
        public static bool Intersects(Circle a, Circle b) { return Vector2.Distance(new Vector2(a.X,a.Y), new Vector2(b.X,b.Y)) < a.Radius + b.Radius; }
        public static bool operator ==(Circle left, Circle right) { return left.Equals(right); }
        public static bool operator !=(Circle left, Circle right) { return !left.Equals(right); }
    }
}

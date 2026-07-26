using System;

namespace MonoGameLibrary.Extensions.Primitives {
    /// <summary>
    /// A simple integer point (X, Y) that is immutable and equatable. 
    /// </summary>
    public readonly struct Point : IEquatable<Point> {
        /// <summary>Gets the X coordinate. </summary>
        public int X { get; }
        
        /// <summary>Gets the Y coordinate. </summary>
        public int Y { get; }
        
        /// <summary>
        /// Initializes a new point with the given coordinates. 
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Point(int x, int y) {
            X = x;
            Y = y;
        }
        
        /// <summary>Indicates whether the current point equals another point. </summary>
        public bool Equals(Point other) {
            return X == other.X && Y == other.Y;
        }
        
        /// <summary>Determines whether the specified object is equal to this point. </summary>
        public override bool Equals(object obj) {
            if (obj is Point other) {
                return Equals(other);
            }
            return false;
        }
        
        /// <summary>Returns a hash code for this point. </summary>
        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }
        
        /// <summary>Compares two points for equality. </summary>
        public static bool operator ==(Point left, Point right) {
            return left.Equals(right);
        }
        
        /// <summary>Compares two points for inequality. </summary>
        public static bool operator !=(Point left, Point right) {
            return !left.Equals(right);
        }
        
        /// <summary>Returns a string representation of the point. </summary>
        public override string ToString() {
            return $"({X}, {Y})";
        }
    }
}
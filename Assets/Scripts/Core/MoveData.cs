/*
 * Define move data for different game mode
 */

using System;

namespace TicTacToe
{
    /// Base Move Data Class
    public abstract class MoveData
    {
        public int X { get; }
        public int Y { get; }
    
        protected MoveData(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    
    /// Move Data for Standard game mode
    public class StandardMove : MoveData
    {
        public StandardMove(int x, int y) : base(x, y) { }
        
        public override string ToString()
        {
            return $"StandardMove({X}, {Y})";
        }
        
        public override bool Equals(object obj)
        {
            if (obj is StandardMove other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
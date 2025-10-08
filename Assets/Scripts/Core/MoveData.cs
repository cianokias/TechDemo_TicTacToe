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
    
    public class UltimateMove : MoveData
    {
        public int BigX { get; }
        public int BigY { get; }
        public int SmallX { get; }
        public int SmallY { get; }
        
        // X and Y represent the absolute position in 9x9 grid
        public UltimateMove(int bigX, int bigY, int smallX, int smallY) 
            : base(bigX * 3 + smallX, bigY * 3 + smallY)
        {
            BigX = bigX;
            BigY = bigY;
            SmallX = smallX;
            SmallY = smallY;
        }
        
        public override string ToString()
        {
            return $"UltimateMove(Big:[{BigX},{BigY}], Small:[{SmallX},{SmallY}])";
        }
        
        public override bool Equals(object obj)
        {
            if (obj is UltimateMove other)
            {
                return BigX == other.BigX && BigY == other.BigY &&
                       SmallX == other.SmallX && SmallY == other.SmallY;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(BigX, BigY, SmallX, SmallY);
        }
    }
}
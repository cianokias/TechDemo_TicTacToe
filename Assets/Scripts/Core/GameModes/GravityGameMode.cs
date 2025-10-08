using System;
using System.Collections.Generic;

namespace TicTacToe
{
    /// Gravity Game Mode, pieces fall down to the bottom
    // Use StandardMove and StandardBoardView
    public class GravityGameMode : StandardGameMode
    {
        public override string GameName => "Tic-Tac-Toe\n(Gravity)";
        
        public override MoveData MakeMove(MoveData move)
        {
            // MoveData type check
            if (!(move is StandardMove standardMove))
            {
                throw new ArgumentException($"Invalid move type. Expected StandardMove, got {move.GetType().Name}");
            }
            
            // Check if the move is valid
            if (!IsValidMove(standardMove))
            {
                return null;
            }

            // Covert move to standard move
            int column = standardMove.X;
            int actualRow = GetLowestEmptyRow(column);
            
            // Fill the board
            Board[column, actualRow] = CurrentPlayerMark;
            MoveCount++;

            // Check if the game is over
            if (CheckWin(column, actualRow)) // if win
            {
                GameState = CurrentPlayerMark == PlayerMark.X ? GameState.Player1Win : GameState.Player2Win;
            }
            else if (CheckDraw())  // if draw
            {
                GameState = GameState.Draw;
            }
            else
            {
                // game is not over, switch player
                CurrentPlayerMark = CurrentPlayerMark == PlayerMark.X ? PlayerMark.O : PlayerMark.X;
            }

            return new StandardMove(column, actualRow);
        }
        
        public override bool IsValidMove(MoveData move)
        {
            // Game is over, no valid move
            if (GameState != GameState.InProgress)
            {
                return false;
            }

            // type check
            if (!(move is StandardMove standardMove))
            {
                return false;
            }

            // board position check
            int column = standardMove.X;
            
            if (column < 0 || column >= BoardSize)
                return false;

            // if the lowest row is empty, it's valid'
            return GetLowestEmptyRow(column) != -1;
        }
        
        public override List<MoveData> GetLegalMoves()
        {
            var moves = new List<MoveData>();

            if (GameState != GameState.InProgress)
            {
                return moves;  // if game is over, no legal moves
            }
            
            for (int x = 0; x < BoardSize; x++)
            {
                int y = GetLowestEmptyRow(x); // find all lowest empty cell
                if (y != -1)
                {
                    moves.Add(new StandardMove(x, y));
                }
            }

            return moves;
        }
        
        private int GetLowestEmptyRow(int column)
        {
            // find the lowest empty row
            for (int row = BoardSize - 1; row >= 0; row--)
            {
                if (Board[column, row] == PlayerMark.Empty)
                {
                    return row;
                }
            }
            return -1; // if it's full
        }

        public override IGameMode Clone()
        {
            var clone = new GravityGameMode
            {
                Board = new PlayerMark[BoardSize, BoardSize]
            };

            // copying board
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    clone.Board[x, y] = this.Board[x, y];
                }
            }
            
            clone.CurrentPlayerMark = this.CurrentPlayerMark;
            clone.GameState = this.GameState;
            clone.MoveCount = this.MoveCount;
            
            return clone;
        }
    }
}
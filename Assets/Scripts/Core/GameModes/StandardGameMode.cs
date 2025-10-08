using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    /// Standard Tic-Tac-Toe Game Mode
    // Use StandardMove and StandardBoardView
    public class StandardGameMode : IGameMode
    {
        protected const int BoardSize = 3;
        
        protected PlayerMark[,] Board;
        protected PlayerMark CurrentPlayerMark;
        protected GameState GameState;
        protected int MoveCount;
        private List<(int, int)> _winningLine;

        // Constructor
        public StandardGameMode()
        {
            Board = new PlayerMark[BoardSize, BoardSize];
            CurrentPlayerMark = PlayerMark.X;
            Reset();
        }

        // IGameMode
        public virtual string GameName => "Tic-Tac-Toe\n(Standard)";

        public GameState CurrentGameState => GameState;

        public int CurrentPlayer 
        { 
            get => CurrentPlayerMark == PlayerMark.X ? 1 : 2;
            set => CurrentPlayerMark = value == 1 ? PlayerMark.X : PlayerMark.O;
        }

        public (int width, int height) GetBoardSize() => (BoardSize, BoardSize);
        
        public void Reset()
        {
            Board = new PlayerMark[BoardSize, BoardSize];

            CurrentPlayerMark = PlayerMark.X;
            GameState = GameState.InProgress;
            _winningLine = null;
            MoveCount = 0;
        }
        
        public virtual MoveData MakeMove(MoveData move)
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
            
            // Fill the board
            Board[standardMove.X, standardMove.Y] = CurrentPlayerMark;
            MoveCount++;

            // Check if the game is over
            if (CheckWin(standardMove.X, standardMove.Y)) // if win
            {
                GameState = CurrentPlayerMark == PlayerMark.X ? GameState.Player1Win : GameState.Player2Win;
            }
            else if (CheckDraw()) // if draw
            {
                GameState = GameState.Draw;
            }
            else
            {
                // game is not over, switch player
                CurrentPlayerMark = CurrentPlayerMark == PlayerMark.X ? PlayerMark.O : PlayerMark.X;
            }

            return move;
        }
        
        public virtual bool IsValidMove(MoveData move)
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
            if (standardMove.X < 0 || standardMove.X >= BoardSize ||
                standardMove.Y < 0 || standardMove.Y >= BoardSize)
            {
                return false;
            }

            // if the position is empty, it's valid'
            return Board[standardMove.X, standardMove.Y] == PlayerMark.Empty;
        }
        
        public virtual List<MoveData> GetLegalMoves()
        {
            var moves = new List<MoveData>();

            if (GameState != GameState.InProgress)
            {
                return moves;  // if game is over, no legal moves
            }

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    if (Board[x, y] == PlayerMark.Empty) // find all empty positions
                    {
                        moves.Add(new StandardMove(x, y));
                    }
                }
            }

            return moves;
        }
        
        public GameResult GetGameResult()
        {
            return new GameResult(GameState, _winningLine);
        }

        // Win Check Functions
        protected bool CheckWin(int lastX, int lastY)
        {
            var player = Board[lastX, lastY];
            
            // same line
            if (CheckLine(0, lastY, 1, 0, player))
            {
                _winningLine = Enumerable.Range(0, BoardSize).Select(x => (x, lastY)).ToList();
                return true;
            }

            // same column
            if (CheckLine(lastX, 0, 0, 1, player))
            {
                _winningLine = Enumerable.Range(0, BoardSize).Select(y => (lastX, y)).ToList();
                return true;
            }

            // diagonal line
            if (lastX == lastY && CheckLine(0, 0, 1, 1, player))
            {
                _winningLine = Enumerable.Range(0, BoardSize).Select(i => (i, i)).ToList();
                return true;
            }

            // another diagonal line
            if (lastX + lastY == BoardSize - 1 && CheckLine(BoardSize - 1, 0, -1, 1, player))
            {
                _winningLine = Enumerable.Range(0, BoardSize).Select(i => (BoardSize - 1 - i, i)).ToList();
                return true;
            }

            return false;
        }
        
        private bool CheckLine(int startX, int startY, int deltaX, int deltaY, PlayerMark playerMark)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                int x = startX + i * deltaX;
                int y = startY + i * deltaY;
                
                if (Board[x, y] != playerMark)
                {
                    return false;
                }
            }
            return true;
        }
        
        protected private bool CheckDraw()
        {
            // if all cells are filled, the game is over
            return MoveCount >= BoardSize * BoardSize;
        }

        public virtual IGameMode Clone()
        {
            var clone = new StandardGameMode
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
        
        // Debug
        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine($"Game: {GameName}");
            result.AppendLine($"State: {GameState}, Current Player: {CurrentPlayerMark}");
            result.AppendLine("Board:");
            
            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    char mark = Board[x, y] switch
                    {
                        PlayerMark.X => 'X',
                        PlayerMark.O => 'O',
                        PlayerMark.Empty => '.',
                        _ => '?'
                    };
                    result.Append($" {mark} ");
                }
                result.AppendLine();
            }
            
            return result.ToString();
        }
    }
}
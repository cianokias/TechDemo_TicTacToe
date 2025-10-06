using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    /// Standard Tic-Tac-Toe Game Mode
    public class StandardGameModes : IGameMode
    {
        private const int BoardSize = 3;
        
        private PlayerMark[,] _board;
        private PlayerMark _currentPlayerMark;
        private GameState _gameState;
        private List<(int, int)> _winningLine;
        private int _moveCount;

        // Constructor
        public StandardGameModes()
        {
            _board = new PlayerMark[BoardSize, BoardSize];
            _currentPlayerMark = PlayerMark.X;
            Reset();
        }

        // IGameMode
        public string GameName => "Tic-Tac-Toe";

        public GameState CurrentGameState => _gameState;

        public int CurrentPlayer => _currentPlayerMark == PlayerMark.X ? 1 : 2;

        public Type GetMoveType() => typeof(StandardMove);

        public (int width, int height) GetBoardSize() => (BoardSize, BoardSize);
        
        public void Reset()
        {
            _board = new PlayerMark[BoardSize, BoardSize];

            _currentPlayerMark = PlayerMark.X;
            _gameState = GameState.InProgress;
            _winningLine = null;
            _moveCount = 0;
        }
        
        public bool MakeMove(MoveData move)
        {
            // MoveData type check
            if (!(move is StandardMove standardMove))
            {
                throw new ArgumentException($"Invalid move type. Expected StandardMove, got {move.GetType().Name}");
            }

            // Check if the move is valid
            if (!IsValidMove(standardMove))
            {
                return false;
            }
            
            // Fill the board
            _board[standardMove.X, standardMove.Y] = _currentPlayerMark;
            _moveCount++;

            // Check if the game is over
            if (CheckWin(standardMove.X, standardMove.Y)) // if win
            {
                _gameState = _currentPlayerMark == PlayerMark.X ? GameState.Player1Win : GameState.Player2Win;
            }
            else if (CheckDraw()) // if draw
            {
                _gameState = GameState.Draw;
            }
            else
            {
                // game is not over, switch player
                _currentPlayerMark = _currentPlayerMark == PlayerMark.X ? PlayerMark.O : PlayerMark.X;
            }

            return true;
        }
        
        public bool IsValidMove(MoveData move)
        {
            // Game is over, no valid move
            if (_gameState != GameState.InProgress)
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
            return _board[standardMove.X, standardMove.Y] == PlayerMark.Empty;
        }
        
        public List<MoveData> GetLegalMoves()
        {
            var moves = new List<MoveData>();

            if (_gameState != GameState.InProgress)
            {
                return moves;  // if game is over, no legal moves
            }

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    if (_board[x, y] == PlayerMark.Empty) // find all empty positions
                    {
                        moves.Add(new StandardMove(x, y));
                    }
                }
            }

            return moves;
        }
        
        public object GetBoardState()
        {
            return (int[,])_board.Clone();
        }
        
        public GameResult GetGameResult()
        {
            return new GameResult(_gameState, _winningLine);
        }

        // Win Check Functions
        private bool CheckWin(int lastX, int lastY)
        {
            var player = _board[lastX, lastY];
            
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
                
                if (_board[x, y] != playerMark)
                {
                    return false;
                }
            }
            return true;
        }
        
        private bool CheckDraw()
        {
            // if all cells are filled, the game is over
            return _moveCount >= BoardSize * BoardSize;
        }

        // Debug
        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine($"Game: {GameName}");
            result.AppendLine($"State: {_gameState}, Current Player: {_currentPlayerMark}");
            result.AppendLine("Board:");
            
            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    char mark = _board[x, y] switch
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
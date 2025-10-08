using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    public class UltimateGameMode : IGameMode
    {
        private const int MetaBoardSize = 3;
        private const int TotalSize = 9;
        
        // 9 small boards
        private StandardGameMode[,] _smallBoards;
        
        // 1 big board
        private StandardGameMode _metaBoard;
        
        // next board coordinate (-1, -1) means any board
        private (int x, int y) _nextBoardConstraint;
        
        private PlayerMark _currentPlayerMark;
        private GameState _gameState;
        private int _moveCount;
        
        public UltimateGameMode()
        {
            Initialize();
            Reset();
        }
        
        private void Initialize()
        {
            _smallBoards = new StandardGameMode[MetaBoardSize, MetaBoardSize];
            
            for (int y = 0; y < MetaBoardSize; y++)
            {
                for (int x = 0; x < MetaBoardSize; x++)
                {
                    _smallBoards[x, y] = new StandardGameMode();
                }
            }

            _metaBoard = new StandardGameMode();
        }
        
        // IGameMode
        public string GameName => "Tic-Tac-Toe\n(Ultimate)";
        public GameState CurrentGameState => _gameState;
        public int CurrentPlayer 
        { 
            get => _currentPlayerMark == PlayerMark.X ? 1 : 2;
            set => _currentPlayerMark = value == 1 ? PlayerMark.X : PlayerMark.O;
        }
        public (int width, int height) GetBoardSize() => (TotalSize, TotalSize);
        
        public void Reset()
        {
            for (int y = 0; y < MetaBoardSize; y++)
            {
                for (int x = 0; x < MetaBoardSize; x++)
                {
                    _smallBoards[x, y].Reset();
                }
            }
            
            _metaBoard.Reset();
            
            _currentPlayerMark = PlayerMark.X;
            _gameState = GameState.InProgress;
            _nextBoardConstraint = (-1, -1);
            _moveCount = 0;
        }
        
        public MoveData MakeMove(MoveData move)
        {
            if (!(move is UltimateMove ultimateMove))
            {
                throw new ArgumentException("Invalid move type for Ultimate mode");
            }
            
            if (!IsValidMove(ultimateMove))
            {
                return null;
            }
            
            var targetBoard = _smallBoards[ultimateMove.BigX, ultimateMove.BigY];
            targetBoard.CurrentPlayer = this.CurrentPlayer;
            var smallMove = new StandardMove(ultimateMove.SmallX, ultimateMove.SmallY);
            
            // make move on target board
            var result = targetBoard.MakeMove(smallMove);
            if (result == null)
            {
                return null;
            }
            
            _moveCount++;
            
            // check if target board is over
            if (targetBoard.CurrentGameState == GameState.Player1Win ||
                targetBoard.CurrentGameState == GameState.Player2Win)
            {
                // mark on meta board
                _metaBoard.CurrentPlayer = this.CurrentPlayer;
                var metaMove = new StandardMove(ultimateMove.BigX, ultimateMove.BigY);
                _metaBoard.MakeMove(metaMove);
                
                // check if meta board is over
                if (_metaBoard.CurrentGameState == GameState.Player1Win ||
                    _metaBoard.CurrentGameState == GameState.Player2Win)
                {
                    _gameState = _currentPlayerMark == PlayerMark.X ? 
                        GameState.Player1Win : GameState.Player2Win;
                }
            }
            
            // check if meta board is draw
            if (_gameState == GameState.InProgress && CheckMetaDraw())
            {
                _gameState = GameState.Draw;
            }
            
            // update state
            if (_gameState == GameState.InProgress)
            {
                UpdateNextBoardConstraint(ultimateMove.SmallX, ultimateMove.SmallY);
                _currentPlayerMark = _currentPlayerMark == PlayerMark.X ? PlayerMark.O : PlayerMark.X;
            }
            
            return ultimateMove;
        }
        
        public bool IsValidMove(MoveData move)
        {
            if (_gameState != GameState.InProgress)
                return false;
            
            if (!(move is UltimateMove ultimateMove))
                return false;
            
            // check if in board
            if (ultimateMove.BigX < 0 || ultimateMove.BigX >= MetaBoardSize ||
                ultimateMove.BigY < 0 || ultimateMove.BigY >= MetaBoardSize)
                return false;
            
            // check if next board constraint is valid
            if (_nextBoardConstraint.x >= 0 && _nextBoardConstraint.y >= 0)
            {
                if (ultimateMove.BigX != _nextBoardConstraint.x ||
                    ultimateMove.BigY != _nextBoardConstraint.y)
                {
                    return false;
                }
            }
            
            // check if target board is valid
            var targetBoard = _smallBoards[ultimateMove.BigX, ultimateMove.BigY];
            if (targetBoard.CurrentGameState != GameState.InProgress)
                return false;
            
            // check if the small move is valid
            var smallMove = new StandardMove(ultimateMove.SmallX, ultimateMove.SmallY);
            return targetBoard.IsValidMove(smallMove);
        }
        
        public List<MoveData> GetLegalMoves()
        {
            var moves = new List<MoveData>();
            
            if (_gameState != GameState.InProgress)
                return moves;
            
            var boardsToCheck = new List<(int, int)>();
            
            if (_nextBoardConstraint == (-1, -1))
            {
                // any board
                for (int y = 0; y < MetaBoardSize; y++)
                {
                    for (int x = 0; x < MetaBoardSize; x++)
                    {
                        if (_smallBoards[x, y].CurrentGameState == GameState.InProgress)
                        {
                            boardsToCheck.Add((x, y));
                        }
                    }
                }
            }
            else
            {
                // specific board
                if (_smallBoards[_nextBoardConstraint.x, _nextBoardConstraint.y]
                    .CurrentGameState == GameState.InProgress)
                {
                    boardsToCheck.Add(_nextBoardConstraint);
                }
            }
            
            foreach (var (bigX, bigY) in boardsToCheck)
            {
                var board = _smallBoards[bigX, bigY];
                var boardMoves = board.GetLegalMoves();
                
                foreach (var boardMove in boardMoves)
                {
                    if (boardMove is StandardMove sm)
                    {
                        moves.Add(new UltimateMove(bigX, bigY, sm.X, sm.Y));
                    }
                }
            }
            
            return moves;
        }
        
        public GameResult GetGameResult()
        {
            return _metaBoard.GetGameResult();
        }
        
        public IGameMode Clone()
        {
            var clone = new UltimateGameMode();
            
            for (int y = 0; y < MetaBoardSize; y++)
            {
                for (int x = 0; x < MetaBoardSize; x++)
                {
                    clone._smallBoards[x, y] = (StandardGameMode)_smallBoards[x, y].Clone();
                }
            }
            
            clone._metaBoard = (StandardGameMode)_metaBoard.Clone();
            clone._currentPlayerMark = _currentPlayerMark;
            clone._gameState = _gameState;
            clone._nextBoardConstraint = _nextBoardConstraint;
            clone._moveCount = _moveCount;
            
            return clone;
        }
        
        // Public helper methods for View
        public GameState GetSmallBoardState(int x, int y)
        {
            if (x >= 0 && x < MetaBoardSize && y >= 0 && y < MetaBoardSize)
            {
                return _smallBoards[x, y].CurrentGameState;
            }
            return GameState.InProgress;
        }
        
        public (int x, int y) GetNextBoardConstraint()
        {
            return _nextBoardConstraint;
        }
        
        // Private helper methods
        private void UpdateNextBoardConstraint(int smallX, int smallY)
        {
            _nextBoardConstraint = (smallX, smallY);
            
            // if the next board is already over, reset the constraint
            if (_smallBoards[smallX, smallY].CurrentGameState != GameState.InProgress)
            {
                _nextBoardConstraint = (-1, -1);
            }
        }
        
        private bool CheckMetaDraw()
        {
            // if meta board is draw, return true
            if (_metaBoard.CurrentGameState == GameState.Draw)
                return true;
                
            // if any small board is not over, return false
            for (int y = 0; y < MetaBoardSize; y++)
            {
                for (int x = 0; x < MetaBoardSize; x++)
                {
                    if (_smallBoards[x, y].CurrentGameState == GameState.InProgress)
                        return false;
                }
            }
            return true;
        }
    }
}
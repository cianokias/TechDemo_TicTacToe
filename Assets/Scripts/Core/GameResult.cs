using System.Collections.Generic;

namespace TicTacToe
{
    /// Store Game Result
    public struct GameResult
    {
        public GameState State { get; }
        public List<(int, int)> WinningLine { get; }  // For UI display

        public GameResult(GameState state, List<(int, int)> winningLine = null)
        {
            State = state;
            WinningLine = winningLine ?? new List<(int, int)>();
        }

        public bool IsGameOver => State != GameState.InProgress && State != GameState.NotStarted;
        public bool HasWinner => State == GameState.Player1Win || State == GameState.Player2Win;
    }
}
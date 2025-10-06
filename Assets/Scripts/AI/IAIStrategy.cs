using System.Collections;

namespace TicTacToe
{
    /// Interface for AI strategies
    public interface IAIStrategy
    {
        /// Get the best move for current game state
        /// Returns The selected move, or null if no valid moves
        MoveData GetBestMove(IGameMode gameMode);
        
        string DifficultyName { get; }
        float ThinkingTime { get; }
    }
}
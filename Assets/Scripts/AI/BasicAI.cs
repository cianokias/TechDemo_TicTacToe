using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    /// Basic AI - Medium difficulty
    // 1. Win if possible
    // 2. Block opponent's winning move
    // 3. Random
    public class BasicAI : IAIStrategy
    {
        public string DifficultyName => "Medium";
        public float ThinkingTime => 0.8f;

        public MoveData GetBestMove(IGameMode gameMode)
        {
            List<MoveData> legalMoves = gameMode.GetLegalMoves();
            
            if (legalMoves == null || legalMoves.Count == 0)
            {
                Debug.LogWarning("BasicAI: No legal moves available");
                return null;
            }

            int currentPlayer = gameMode.CurrentPlayer;
            int opponent = currentPlayer == 1 ? 2 : 1;

            // Priority 1: Check if we can win with any move
            MoveData winningMove = FindWinningMove(gameMode, legalMoves, currentPlayer);
            if (winningMove != null)
            {
                Debug.Log($"BasicAI: Found winning move!");
                return winningMove;
            }

            // Priority 2: Check if we need to block opponent's winning move
            MoveData blockingMove = FindBlockingMove(gameMode, legalMoves, opponent);
            if (blockingMove != null)
            {
                Debug.Log($"BasicAI: Blocking opponent's winning move!");
                return blockingMove;
            }

            // Priority 3: Random
            int randomIndex = Random.Range(0, legalMoves.Count);
            Debug.Log($"BasicAI: Choosing random move");
            return legalMoves[randomIndex];
        }
        
        // Find a move that would result in an immediate win
        private MoveData FindWinningMove(IGameMode gameMode, List<MoveData> legalMoves, int currentPlayer)
        {
            foreach (var move in legalMoves)
            {
                // Clone the game state to simulate the move
                IGameMode simulation = gameMode.Clone();
                
                // Try the move
                if (simulation.MakeMove(move) != null)
                {
                    // Check if this move results in a win for current player
                    if ((currentPlayer == 1 && simulation.CurrentGameState == GameState.Player1Win) ||
                        (currentPlayer == 2 && simulation.CurrentGameState == GameState.Player2Win))
                    {
                        return move;
                    }
                }
            }
            
            return null;
        }
        
        // Find a move that blocks the opponent from winning on their next turn
        private MoveData FindBlockingMove(IGameMode gameMode, List<MoveData> legalMoves, int opponent)
        {
            foreach (var move in legalMoves)
            {
                // Find another legal move to make first (to switch turns)
                MoveData otherMove = null;
                foreach (var m in legalMoves)
                {
                    if (!m.Equals(move))  // Make sure it's a different position
                    {
                        otherMove = m;
                        break;
                    }
                }
                
                if (otherMove != null)
                {
                    IGameMode sim = gameMode.Clone();
                    sim.MakeMove(otherMove); // Our turn, play elsewhere
                    
                    if (sim.CurrentGameState == GameState.InProgress)
                    {
                        // Now it's opponent's turn, see if they can win with 'move'
                        if (sim.IsValidMove(move) && sim.MakeMove(move) != null)
                        {
                            if ((opponent == 1 && sim.CurrentGameState == GameState.Player1Win) ||
                                (opponent == 2 && sim.CurrentGameState == GameState.Player2Win))
                            {
                                return move; // We should block this position
                            }
                        }
                    }
                }
                else
                {
                    // Special case: only one move left (move == the only legal move)
                    // Just take it
                    return move;
                }
            }
            
            return null;
        }
    }
}
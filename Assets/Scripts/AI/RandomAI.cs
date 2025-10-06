using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    /// Random AI - picks a random legal move
    public class RandomAI : IAIStrategy
    {
        public string DifficultyName => "Easy";
        public float ThinkingTime => 0.5f;

        public MoveData GetBestMove(IGameMode gameMode)
        {
            // Get all legal moves
            List<MoveData> legalMoves = gameMode.GetLegalMoves();
            
            // If no legal moves, return null
            if (legalMoves == null || legalMoves.Count == 0)
            {
                Debug.LogWarning("RandomAI: No legal moves available");
                return null;
            }
            
            // Pick a random move
            int randomIndex = Random.Range(0, legalMoves.Count);
            MoveData selectedMove = legalMoves[randomIndex];
            
            Debug.Log($"RandomAI selected move: {selectedMove}");
            return selectedMove;
        }
    }
}
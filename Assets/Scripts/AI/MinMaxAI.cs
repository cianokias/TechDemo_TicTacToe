using System;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    /// Minimax AI - Unbeatable for standard Tic-Tac-Toe
    public class MinMaxAI : IAIStrategy
    {
        public string DifficultyName => "Impossible";
        public float ThinkingTime => 1.2f;
        
        private int _maxDepth = 9;
        private int _aiPlayer;
        private int _humanPlayer;
        private int _nodesEvaluated;
        
        public MoveData GetBestMove(IGameMode gameMode)
        {
            var legalMoves = gameMode.GetLegalMoves();
            if (legalMoves == null || legalMoves.Count == 0)
            {
                return null;
            }
            
            _aiPlayer = gameMode.CurrentPlayer;
            _humanPlayer = _aiPlayer == 1 ? 2 : 1;
            _nodesEvaluated = 0;
            if (gameMode is UltimateGameMode) _maxDepth = 4;
            
            MoveData bestMove = null;
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            
            // For every move, evaluate it and choose the best one
            foreach (var move in legalMoves)
            {
                IGameMode simulation = gameMode.Clone();
                simulation.CurrentPlayer = _aiPlayer;
                
                if (simulation.MakeMove(move) != null)
                {
                    // minmax
                    int score = MinMax(simulation, 0, false, alpha, beta);
                    
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                    
                    alpha = Math.Max(alpha, bestScore);
                }
            }
            
            Debug.Log($"MinmaxAI evaluated {_nodesEvaluated} nodes, best score: {bestScore}");
            return bestMove;
        }
        
        private int MinMax(IGameMode gameMode, int depth, bool isMaximizing, int alpha, int beta)
        {
            _nodesEvaluated++;
            
            // End of the game or max depth reached
            if (gameMode.CurrentGameState != GameState.InProgress || depth >= _maxDepth)
            {
                return EvaluatePosition(gameMode, depth);
            }
            
            var legalMoves = gameMode.GetLegalMoves();
            if (legalMoves.Count == 0)
            {
                return EvaluatePosition(gameMode, depth);
            }
            
            if (isMaximizing)
            {
                int maxScore = int.MinValue;
                
                foreach (var move in legalMoves)
                {
                    IGameMode simulation = gameMode.Clone();
                    simulation.CurrentPlayer = _aiPlayer;
                    
                    if (simulation.MakeMove(move) != null)
                    {
                        int score = MinMax(simulation, depth + 1, false, alpha, beta);
                        maxScore = Math.Max(maxScore, score);
                        alpha = Math.Max(alpha, maxScore);
                        
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                
                return maxScore;
            }
            else
            {
                int minScore = int.MaxValue;
                
                foreach (var move in legalMoves)
                {
                    IGameMode simulation = gameMode.Clone();
                    simulation.CurrentPlayer = _humanPlayer;
                    
                    if (simulation.MakeMove(move) != null)
                    {
                        int score = MinMax(simulation, depth + 1, true, alpha, beta);
                        minScore = Math.Min(minScore, score);
                        beta = Math.Min(beta, minScore);
                        
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
                
                return minScore;
            }
        }
        
        private int EvaluatePosition(IGameMode gameMode, int depth)
        {
            var state = gameMode.CurrentGameState;
            
            if (state == GameState.Player1Win)
            {
                return _aiPlayer == 1 ? (1000 - depth) : (-1000 + depth);
            }
            else if (state == GameState.Player2Win)
            {
                return _aiPlayer == 2 ? (1000 - depth) : (-1000 + depth);
            }
            else if (state == GameState.Draw)
            {
                return 0;
            }
            
            if (gameMode is UltimateGameMode ultimateGame)
            {
                return EvaluateUltimate(ultimateGame);
            }
            
            return 0;
        }
        

        /// Ultimate game mode specific evaluation functions
        private int EvaluateUltimate(UltimateGameMode gameMode)
        {
            int score = 0;

            // 1. evaluate meta lines
            score += EvaluateMetaLines(gameMode) * 50;
            
            // 2. evaluate next board constraint
            int[,] boardValues = {
                {30, 20, 30},  // corner 30
                {20, 40, 20},  // center 40
                {30, 20, 30}   // side 20
            };
            
            var constraint = gameMode.GetNextBoardConstraint();
            if (constraint.x >= 0 && constraint.y >= 0)
            {
                var constrainedBoardState = gameMode.GetSmallBoardState(constraint.x, constraint.y);
                if (constrainedBoardState == GameState.InProgress)
                {
                    int boardValue = boardValues[constraint.x, constraint.y];
                    score += (_aiPlayer == gameMode.CurrentPlayer ? boardValue / 2 : -boardValue / 2);
                }
            }
            
            return score;
        }
        
        /// Evaluate meta lines
        private int EvaluateMetaLines(UltimateGameMode gameMode)
        {
            int score = 0;
            
            // Check each meta line
            for (int i = 0; i < 3; i++)
            {
                score += EvaluateMetaLine(gameMode, i, 0, 0, 1);
                score += EvaluateMetaLine(gameMode, 0, i, 1, 0);
            }
            // diagonal lines
            score += EvaluateMetaLine(gameMode, 0, 0, 1, 1);
            score += EvaluateMetaLine(gameMode, 2, 0, -1, 1);
            
            return score;
        }
        
        private int EvaluateMetaLine(UltimateGameMode gameMode, int startX, int startY, int deltaX, int deltaY)
        {
            int aiCount = 0;
            int humanCount = 0;
            int emptyCount = 0;
            
            for (int i = 0; i < 3; i++)
            {
                int x = startX + i * deltaX;
                int y = startY + i * deltaY;
                var state = gameMode.GetSmallBoardState(x, y);
                
                if (state == GameState.Player1Win)
                {
                    if (_aiPlayer == 1) aiCount++;
                    else humanCount++;
                }
                else if (state == GameState.Player2Win)
                {
                    if (_aiPlayer == 2) aiCount++;
                    else humanCount++;
                }
                else if (state == GameState.InProgress)
                {
                    emptyCount++;
                }
            }
            
            if (humanCount == 0)
            {
                if (aiCount == 2) return 50;
                if (aiCount == 1) return 10;
            }
            else if (aiCount == 0)
            {
                if (humanCount == 2) return -50;
                if (humanCount == 1) return -10;
            }
            
            return 0;
        }
    }
}
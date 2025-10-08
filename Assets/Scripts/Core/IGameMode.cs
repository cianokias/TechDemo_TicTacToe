using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public interface IGameMode
    {
        // Core Game Logic
        MoveData MakeMove(MoveData move);
        bool IsValidMove(MoveData move);
        List<MoveData> GetLegalMoves();
        void Reset();

        // Game State
        GameState CurrentGameState { get; }
        int CurrentPlayer { get; set; }
        GameResult GetGameResult();

        // Metadata
        string GameName { get; }
        (int width, int height) GetBoardSize();
        
        // Clone for AI simulation
        IGameMode Clone();
    }
}
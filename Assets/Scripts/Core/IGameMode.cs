using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public interface IGameMode
    {
        // Core Game Logic
        bool MakeMove(MoveData move);
        bool IsValidMove(MoveData move);
        List<MoveData> GetLegalMoves();
        void Reset();

        // Game State
        GameState CurrentGameState { get; }
        int CurrentPlayer { get; }
        GameResult GetGameResult();

        // Display
        object GetBoardState();

        // Metadata
        string GameName { get; }
        Type GetMoveType();
        (int width, int height) GetBoardSize();
    }
}
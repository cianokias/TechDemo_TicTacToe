/*
 * Define all the enums used in the game
 */

namespace TicTacToe
{
    public enum GameState
    {
        NotStarted,
        InProgress,
        Player1Win,
        Player2Win,
        Draw
    }
    
    public enum PlayerMark
    {
        Empty = 0,     // Empty cell
        X = 1,         // Player1 (X)
        O = 2          // Player2 (O)
    }
    
    public enum GameModeType
    {
        Standard
    }
}
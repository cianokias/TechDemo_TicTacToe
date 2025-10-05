namespace TicTacToe
{
    public interface IBoardView
    {
        void Initialize(IGameMode gameMode);
        void OnCellClicked(int x, int y);
        void UpdateCell(int x, int y, PlayerMark mark);
        void HighlightWinningLine(System.Collections.Generic.List<(int, int)> winningCells);
        void SetInteractable(bool interactable);
        void Reset();
        void Cleanup();
    }
}
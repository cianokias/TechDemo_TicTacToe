using UnityEngine;
using System.Collections.Generic;

namespace TicTacToe
{
    public class StandardBoardView : MonoBehaviour, IBoardView
    {
        [Header("Layout Settings")]
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private float cellSpacing = 0.1f;
        
        private CellView[,] _cells;
        private GameObject _cellPrefab;
        private (int x, int y) _lastAIMovePos = (-1, -1);
        
        public void Initialize(IGameMode gameMode, GameObject cellPrefab)
        {
            _cellPrefab = cellPrefab;
            
            var (width, height) = gameMode.GetBoardSize();
            CreateBoard(width, height);
        }
        
        private void CreateBoard(int width, int height)
        {
            // Clean up old board
            Cleanup();
            
            _cells = new CellView[width, height];
            
            // Calculate board top left position
            float boardWidth = width * cellSize + (width - 1) * cellSpacing;
            float boardHeight = height * cellSize + (height - 1) * cellSpacing;
            Vector3 startPos = new Vector3(-boardWidth / 2 + cellSize / 2, 
                                          boardHeight / 2 - cellSize / 2, 
                                          0);
            
            // Generate cells
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate cell position
                    Vector3 localPosition = startPos + new Vector3(
                        x * (cellSize + cellSpacing),
                        -y * (cellSize + cellSpacing),
                        0
                    );
                    
                    // Create cell GameObject and add it to the board
                    GameObject cellObj = Instantiate(_cellPrefab, transform);
                    cellObj.transform.localPosition = localPosition;
                    cellObj.name = $"Cell_{x}_{y}";
                    
                    CellView cellView = cellObj.GetComponent<CellView>();
                    if (cellView == null)
                    {
                        Debug.LogError("Cell prefab must have CellView component!");
                        continue;
                    }
                    
                    cellView.Initialize(x, y, this);
                    _cells[x, y] = cellView;
                }
            }
            
            Debug.Log($"Created {width}x{height} board");
        }
        
        public void OnCellClicked(int x, int y)
        {
            MoveData move = new StandardMove(x, y);
            GameManager.Instance?.OnPlayerInput(move);
        }
        
        public void UpdateCell(int x, int y, PlayerMark mark)
        {
            if (IsValidPosition(x, y))
            {
                _cells[x, y].SetMark(mark);
            }
        }
        
        public void HighlightWinningLine(List<(int, int)> winningCells)
        {
            foreach (var (x, y) in winningCells)
            {
                if (IsValidPosition(x, y))
                {
                    _cells[x, y].SetAsWinningCell();
                }
            }
        }
        
        public void HighlightLastMove(int x, int y)
        {
            if (_lastAIMovePos.x >= 0 && _lastAIMovePos.y >= 0)
            {
                if (IsValidPosition(_lastAIMovePos.x, _lastAIMovePos.y))
                {
                    _cells[_lastAIMovePos.x, _lastAIMovePos.y].SetAsLastAIMove(false);
                }
            }
            
            if (IsValidPosition(x, y))
            {
                _cells[x, y].SetAsLastAIMove(true);
                _lastAIMovePos = (x, y);
            }
        }
        
        public void SetInteractable(bool interactable)
        {
            if (_cells == null) return;
            
            foreach (var cell in _cells)
            {
                if (cell != null)
                {
                    cell.SetInteractable(interactable);
                }
            }
        }
        
        public void Reset()
        {
            if (_cells == null) return;
            
            foreach (var cell in _cells)
            {
                if (cell != null)
                {
                    cell.Reset();
                }
            }
        }
        
        public void Cleanup()
        {
            if (_cells != null)
            {
                foreach (var cell in _cells)
                {
                    if (cell != null && cell.gameObject != null)
                    {
                        Destroy(cell.gameObject);
                    }
                }
                _cells = null;
            }
        }
        
        private bool IsValidPosition(int x, int y)
        {
            return _cells != null && 
                   x >= 0 && x < _cells.GetLength(0) && 
                   y >= 0 && y < _cells.GetLength(1);
        }
        
        void OnDestroy()
        {
            Cleanup();
        }
    }
}
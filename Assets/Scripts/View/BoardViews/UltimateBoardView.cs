using UnityEngine;
using System.Collections.Generic;

namespace TicTacToe
{
    public class UltimateBoardView : MonoBehaviour, IBoardView
    {
        [Header("Layout Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float cellSpacing = 0.05f;
        [SerializeField] private float boardSpacing = 0.2f;
        
        private CellView[,] _cells;  // 9x9格子
        private CellView[,] _metaCells;  // 3x3覆盖层
        private GameObject _cellPrefab;
        private UltimateGameMode _gameMode;
        private (int x, int y) _lastAIMovePos = (-1, -1);
        
        public void Initialize(IGameMode gameMode, GameObject cellPrefab)
        {
            if (!(gameMode is UltimateGameMode ultimateMode))
            {
                Debug.LogError("UltimateBoardView requires UltimateGameMode!");
                return;
            }
            
            _gameMode = ultimateMode;
            _cellPrefab = cellPrefab;
            
            CreateBoard();
        }
        
        private void CreateBoard()
        {
            Cleanup();
            
            Create9X9Grid();
            CreateMetaCells();
            
            UpdateAllBoardsInteractability();
        }
        
        private void Create9X9Grid()
        {
            _cells = new CellView[9, 9];
            
            float totalWidth = 9 * cellSize + 8 * cellSpacing + 2 * boardSpacing;
            float totalHeight = totalWidth;
    
            // start point
            Vector3 startPos = new Vector3(-totalWidth / 2 + cellSize / 2, 
                totalHeight / 2 - cellSize / 2, 0);
    
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    float posX = x * (cellSize + cellSpacing);
                    float posY = -y * (cellSize + cellSpacing);
                    
                    if (x >= 3) posX += boardSpacing;
                    if (x >= 6) posX += boardSpacing;
                    if (y >= 3) posY -= boardSpacing;
                    if (y >= 6) posY -= boardSpacing;
            
                    Vector3 localPosition = startPos + new Vector3(posX, posY, 0);
            
                    GameObject cellObj = Instantiate(_cellPrefab, transform);
                    cellObj.transform.localPosition = localPosition;
                    cellObj.transform.localScale = Vector3.one * cellSize;
                    cellObj.name = $"Cell_{x}_{y}";
            
                    CellView cellView = cellObj.GetComponent<CellView>();
                    cellView.Initialize(x, y, this);
                    _cells[x, y] = cellView;
                }
            }
        }
        
        private void CreateMetaCells()
        {
            _metaCells = new CellView[3, 3];
    
            float metaCellSize = 3 * cellSize + 2 * cellSpacing;
            float totalWidth = 9 * cellSize + 8 * cellSpacing + 2 * boardSpacing;
            
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    int firstCellX = x * 3;
                    int firstCellY = y * 3;
                    
                    float posX = firstCellX * (cellSize + cellSpacing);
                    float posY = -firstCellY * (cellSize + cellSpacing);
                    
                    if (x >= 1) posX += boardSpacing;
                    if (x >= 2) posX += boardSpacing;
                    if (y >= 1) posY -= boardSpacing;
                    if (y >= 2) posY -= boardSpacing;
                    
                    posX += metaCellSize / 2;
                    posY -= metaCellSize / 2;
            
                    Vector3 position = new Vector3(
                        -totalWidth / 2 + posX,
                        totalWidth / 2 + posY,
                        -0.1f
                    );
            
                    GameObject metaCellObj = Instantiate(_cellPrefab, transform);
                    metaCellObj.name = $"MetaCell_{x}_{y}";
                    metaCellObj.transform.localPosition = position;
                    metaCellObj.transform.localScale = Vector3.one * metaCellSize;
                    
                    Collider2D[] colliders = metaCellObj.GetComponentsInChildren<Collider2D>();
                    foreach (var collider in colliders)
                    {
                        Destroy(collider);
                    }
            
                    CellView metaCell = metaCellObj.GetComponent<CellView>();
                    metaCell.Initialize(x, y, null);
                    metaCell.SetInteractable(false);
                    
                    var backgroundRenderer = metaCellObj.transform.Find("Background")?.GetComponent<SpriteRenderer>();
                    if (backgroundRenderer != null)
                    {
                        backgroundRenderer.color = new Color(1, 1, 1, 0);
                        backgroundRenderer.sortingOrder = 2;
                    }
            
                    var pieceRenderer = metaCellObj.transform.Find("Piece")?.GetComponent<SpriteRenderer>();
                    if (pieceRenderer != null)
                    {
                        pieceRenderer.sortingOrder = 3;
                    }
            
                    _metaCells[x, y] = metaCell;
                }
            }
        }
        
        public void OnCellClicked(int x, int y)
        {
            int bigX = x / 3;
            int bigY = y / 3;
            int smallX = x % 3;
            int smallY = y % 3;
            
            var move = new UltimateMove(bigX, bigY, smallX, smallY);
            GameManager.Instance?.OnPlayerInput(move);
        }
        
        public void UpdateCell(int x, int y, PlayerMark mark)
        {
            // 更新9x9格子
            if (IsValidPosition(x, y))
            {
                _cells[x, y].SetMark(mark);
            }
            
            // 更新元棋盘覆盖层
            int bigX = x / 3;
            int bigY = y / 3;
            UpdateMetaCell(bigX, bigY);
            
            // 更新可交互性
            UpdateAllBoardsInteractability();
        }
        
        private void UpdateMetaCell(int bigX, int bigY)
        {
            if (_gameMode == null || _metaCells == null) return;
            
            var boardState = _gameMode.GetSmallBoardState(bigX, bigY);
            var metaCell = _metaCells[bigX, bigY];
            if (metaCell == null) return;
            
            var backgroundRenderer = metaCell.transform.Find("Background")?.GetComponent<SpriteRenderer>();
            
            switch (boardState)
            {
                case GameState.Player1Win:
                    metaCell.SetMark(PlayerMark.X);
                    SetBackgroundColor(backgroundRenderer, new Color(1, 1, 1, 0.5f));
                    break;
                    
                case GameState.Player2Win:
                    metaCell.SetMark(PlayerMark.O);
                    SetBackgroundColor(backgroundRenderer, new Color(1, 1, 1, 0.5f));
                    break;
                    
                case GameState.Draw:
                    metaCell.SetMark(PlayerMark.Empty);
                    SetBackgroundColor(backgroundRenderer, new Color(1, 1, 1, 0.5f));
                    break;
                    
                default:
                    metaCell.SetMark(PlayerMark.Empty);
                    SetBackgroundColor(backgroundRenderer, new Color(1, 1, 1, 0));
                    break;
            }
        }
        
        private void UpdateAllBoardsInteractability()
        {
            if (_gameMode == null) return;
            
            var constraint = _gameMode.GetNextBoardConstraint();
            bool hasConstraint = (constraint.x >= 0 && constraint.y >= 0);
            
            for (int bigY = 0; bigY < 3; bigY++)
            {
                for (int bigX = 0; bigX < 3; bigX++)
                {
                    var boardState = _gameMode.GetSmallBoardState(bigX, bigY);
                    bool isConstrained = (constraint.x == bigX && constraint.y == bigY);
                    
                    // 更新9x9格子的可交互性
                    bool canPlay = boardState == GameState.InProgress && 
                                  (!hasConstraint || isConstrained);
                    
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            int absoluteX = bigX * 3 + x;
                            int absoluteY = bigY * 3 + y;
                            _cells[absoluteX, absoluteY].SetInteractable(canPlay);
                        }
                    }
                }
            }
        }
        
        public void HighlightWinningLine(List<(int, int)> winningCells)
        {
            if (winningCells != null)
            {
                foreach (var (x, y) in winningCells)
                {
                    if (x >= 0 && x < 3 && y >= 0 && y < 3 && _metaCells[x, y] != null)
                    {
                        var backgroundRenderer = _metaCells[x, y].transform.Find("Background")?.GetComponent<SpriteRenderer>();
                        SetBackgroundColor(backgroundRenderer, new Color(0.3f, 1f, 0.2f, 0.5f));
                    }
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
            if (!interactable)
            {
                if (_cells != null)
                {
                    foreach (var cell in _cells)
                    {
                        if (cell != null)
                            cell.SetInteractable(false);
                    }
                }
            }
            else
            {
                UpdateAllBoardsInteractability();
            }
        }
        
        public void Reset()
        {
            if (_cells != null)
            {
                foreach (var cell in _cells)
                {
                    if (cell != null)
                        cell.Reset();
                }
            }
            
            if (_metaCells != null)
            {
                foreach (var metaCell in _metaCells)
                {
                    if (metaCell != null)
                    {
                        metaCell.Reset();
                        var backgroundRenderer = metaCell.transform.Find("Background")?.GetComponent<SpriteRenderer>();
                        SetBackgroundColor(backgroundRenderer, new Color(1, 1, 1, 0));
                    }
                }
            }
            
            UpdateAllBoardsInteractability();
        }
        
        public void Cleanup()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            _cells = null;
            _metaCells = null;
        }
        
        private bool IsValidPosition(int x, int y)
        {
            return _cells != null && x >= 0 && x < 9 && y >= 0 && y < 9;
        }
        
        private void SetBackgroundColor(SpriteRenderer renderer, Color color)
        {
            if (renderer != null)
                renderer.color = color;
        }
        
        void OnDestroy()
        {
            Cleanup();
        }
    }
}

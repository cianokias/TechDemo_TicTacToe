using UnityEngine;

namespace TicTacToe
{
    public class CellView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        
        [Header("Sprites")]
        [SerializeField] private Sprite xSprite;
        [SerializeField] private Sprite oSprite;
        
        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        [SerializeField] private Color winColor = new Color(0.3f, 1f, 0.2f, 1f);
        [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.6f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        
        // Position in the board
        private int _x;
        private int _y;
        
        // State
        private bool _isOccupied = false;
        private bool _isInteractable = true;
        private bool _isPartOfWinningLine = false;
        private bool _isLastAIMove = false;
        
        // Reference to the board
        private IBoardView _boardView;
        
        void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = transform.Find("Piece")?.GetComponent<SpriteRenderer>();
            if (backgroundRenderer == null)
                backgroundRenderer = transform.Find("Background")?.GetComponent<SpriteRenderer>();
        }
        
        public void Initialize(int x, int y, IBoardView boardView)
        {
            _x = x;
            _y = y;
            _boardView = boardView;
            Reset();
        }
        
        public void SetMark(PlayerMark mark)
        {
            _isOccupied = true;
            
            switch (mark)
            {
                case PlayerMark.X:
                    spriteRenderer.sprite = xSprite;
                    break;
                case PlayerMark.O:
                    spriteRenderer.sprite = oSprite;
                    break;
                case PlayerMark.Empty:
                    spriteRenderer.sprite = null;
                    _isOccupied = false;
                    break;
            }
        }
        
        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            UpdateVisualState();
        }

        public void SetAsLastAIMove(bool isLast)
        {
            _isLastAIMove = isLast;
            UpdateVisualState();
        }
        
        void UpdateVisualState()
        {
            if (_isPartOfWinningLine)
            {
                backgroundRenderer.color = winColor;
            }
            else if (_isLastAIMove)
            {
                backgroundRenderer.color = highlightColor;
            }
            else if (!_isInteractable)
            {
                backgroundRenderer.color = disabledColor;
            }
            else
            {
                backgroundRenderer.color = normalColor;
            }
        }
        
        public void SetAsWinningCell()
        {
            _isPartOfWinningLine = true;
            backgroundRenderer.color = winColor;
        }
        
        public void Reset()
        {
            _isOccupied = false;
            _isInteractable = true;
            _isPartOfWinningLine = false;
            _isLastAIMove = false;
            spriteRenderer.sprite = null;
            backgroundRenderer.color = normalColor;
        }
        
        void OnMouseDown()
        {
            if (_isInteractable && !_isOccupied)
            {
                _boardView?.OnCellClicked(_x, _y);
            }
        }
        
        void OnMouseEnter()
        {
            if (_isInteractable && !_isOccupied && !_isLastAIMove)
            {
                backgroundRenderer.color = hoverColor;
            }
        }
        
        void OnMouseExit()
        {
            if (!_isPartOfWinningLine && _isInteractable && !_isLastAIMove)
            {
                backgroundRenderer.color = _isInteractable ? normalColor : disabledColor;
            }
        }
    }
}

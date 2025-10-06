using UnityEngine;

namespace TicTacToe
{
    public class GameManager : MonoBehaviour
    {
        // Singleton
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        [SerializeField] private GameModeType gameMode = GameModeType.Standard;
        
        [Header("Board Settings")]
        [SerializeField] private GameObject cellPrefab;
        
        // Core components
        private IGameMode _currentGame;
        private IBoardView _boardView;
        
        // Game state
        private bool _isGameOver = false;
        
        void Awake()
        {
            Instance = this;
        }
        
        void Start()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            // 1. Create GameMode
            _currentGame = CreateGameMode(gameMode);
            
            // 2. Create corresponding BoardView
            _boardView = CreateBoardView(gameMode);
            
            // 3. Initialize BoardView
            _boardView.Initialize(_currentGame, cellPrefab);
            
            // 4. Start new game
            StartNewGame();
        }
                
        private IGameMode CreateGameMode(GameModeType type)
        {
            switch (type)
            {
                case GameModeType.Standard:
                    return new StandardGameModes();
                
                // more case in the future
                
                default:
                    Debug.LogWarning($"Game mode {type} not implemented, using Standard");
                    return new StandardGameModes();
            }
        }
        
        private IBoardView CreateBoardView(GameModeType type)
        {
            // Cleanup old BoardView GameObject
            if (_boardView != null)
            {
                _boardView.Cleanup();
                if (_boardView is MonoBehaviour oldView)
                {
                    Destroy(oldView.gameObject);
                }
            }
            
            // Create new BoardView GameObject
            GameObject boardViewObj = new GameObject($"{type}BoardView")
            {
                transform =
                {
                    localPosition = Vector3.zero
                }
            };

            // Add BoardView component based on game mode
            IBoardView newBoardView = null;
            switch (type)
            {
                case GameModeType.Standard:
                    newBoardView = boardViewObj.AddComponent<StandardBoardView>();
                    break;
                
                // more case in the future
                
                default:
                    Debug.LogError($"No board view implementation for game mode: {type}");
                    newBoardView = boardViewObj.AddComponent<StandardBoardView>();
                    break;
            }
            
            return newBoardView;
        }

        public void StartNewGame()
        {
            _isGameOver = false;
            
            // reset game logic
            _currentGame.Reset();
            
            // reset view
            _boardView.Reset();
            _boardView.SetInteractable(true);
            
            // update ui
            UpdateDisplay();
        }
        
        // Handle Player Input (from board view)
        public void OnPlayerInput(MoveData move)
        {
            if (_isGameOver)
                return;
            
            // Mark the current player
            int currentPlayer = _currentGame.CurrentPlayer;
            
            // Try to make move
            if (_currentGame.MakeMove(move))
            {
                // Update view
                PlayerMark mark = currentPlayer == 1 ? PlayerMark.X : PlayerMark.O;
                _boardView.UpdateCell(move.X, move.Y, mark);
                
                // Check if game is over
                if (_currentGame.CurrentGameState != GameState.InProgress)
                {
                    HandleGameOver();
                }
                else
                {
                    UpdateDisplay();
                }
            }
            else
            {
                // illegal move
                Debug.Log($"Invalid move at ({move.X}, {move.Y})");
            }
        }
        
        private void HandleGameOver()
        {
            _isGameOver = true;
            
            // no more input
            _boardView.SetInteractable(false);
            
            // get the result
            GameResult result = _currentGame.GetGameResult();
            
            // highlight winning line
            if (result is { HasWinner: true, WinningLine: { Count: > 0 } })
            {
                _boardView.HighlightWinningLine(result.WinningLine);
            }
            
            // show result
            string message = GetGameOverMessage(result.State);
            Debug.Log($"Game Over: {message}");
        }
        
        private string GetGameOverMessage(GameState state)
        {
            return state switch
            {
                GameState.Player1Win => "Player 1 (X) Wins!",
                GameState.Player2Win => "Player 2 (O) Wins!",
                GameState.Draw => "It's a Draw!",
                _ => "Game Over"
            };
        }
        
        private void UpdateDisplay()
        {
            if (_currentGame.CurrentGameState == GameState.InProgress)
            {
                string currentPlayerText = _currentGame.CurrentPlayer == 1 ? 
                    "Player 1 (X)" : "Player 2 (O)";
                string message = $"{currentPlayerText}'s Turn";
                Debug.Log(message);
            }
        }
        
        // For UI
        public void RestartGame()
        {
            StartNewGame();
        }
        
        public IGameMode GetCurrentGame()
        {
            return _currentGame;
        }
        
        public bool IsGameOver()
        {
            return _isGameOver;
        }
        
        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            
            if (_boardView != null)
            {
                _boardView.Cleanup();
            }
        }
    }
}

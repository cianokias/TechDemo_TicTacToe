using UnityEngine;

namespace TicTacToe
{
    public class CameraAutoFit : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float padding = 1.0f;
        [SerializeField] private bool autoFitOnStart = true;
        [SerializeField] private float smoothSpeed = 5f;
        
        private float _targetOrthographicSize;
        private bool _isTransitioning = false;
        
        void Awake()
        {
            if (targetCamera == null)
                targetCamera = GetComponent<Camera>();
                
            if (targetCamera == null)
                targetCamera = Camera.main;
        }
        
        void Start()
        {
            if (autoFitOnStart)
            {
                // delay 1 frame to allow the board to be initialized
                Invoke(nameof(FitToBoard), 0.1f);
            }
        }
        
        void Update()
        {
            // transitions
            if (_isTransitioning && targetCamera != null)
            {
                if (smoothSpeed <= 0)
                {
                    targetCamera.orthographicSize = _targetOrthographicSize;
                    _isTransitioning = false;
                }
                else
                {
                    float lerpFactor = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            
                    targetCamera.orthographicSize = Mathf.Lerp(
                        targetCamera.orthographicSize,
                        _targetOrthographicSize,
                        lerpFactor
                    );
            
                    if (Mathf.Abs(targetCamera.orthographicSize - _targetOrthographicSize) < 0.01f)
                    {
                        targetCamera.orthographicSize = _targetOrthographicSize;
                        _isTransitioning = false;
                    }
                }
            }
        }
        
        public void FitToBoard()
        {
            if (targetCamera == null)
            {
                Debug.LogError("No camera assigned!");
                return;
            }
            
            // Find board view
            IBoardView boardView = FindObjectOfType<StandardBoardView>();
            if (boardView == null)
            {
                Debug.LogWarning("No board view found!");
                return;
            }
            
            // get board transform
            Transform boardTransform = (boardView as MonoBehaviour)?.transform;
            if (boardTransform == null)
                return;
            
            // calculate bounds
            Bounds bounds = CalculateBounds(boardTransform);
            
            if (bounds.size == Vector3.zero)
            {
                Debug.LogWarning("Board has no size!");
                return;
            }
            
            FitCameraToBounds(bounds);
        }
        
        private Bounds CalculateBounds(Transform boardTransform)
        {
            // get all cell's renderers
            Renderer[] renderers = boardTransform.GetComponentsInChildren<Renderer>();

            // calculate bounds
            Bounds bounds = new Bounds(boardTransform.position, Vector3.zero);
            foreach (Renderer r in renderers)
            {
                bounds.Encapsulate(r.bounds);
            }
            
            return bounds;
        }
        
        private void FitCameraToBounds(Bounds bounds)
        {
            // add padding
            bounds.Expand(padding);
            
            // move camera to center
            Vector3 newPosition = bounds.center;
            newPosition.z = targetCamera.transform.position.z;
            targetCamera.transform.position = newPosition;
            
            // calculate orthographic size
            float screenRatio = targetCamera.aspect;
            float targetRatio = bounds.size.x / bounds.size.y;
    
            if (screenRatio >= targetRatio)
            {
                _targetOrthographicSize = bounds.size.y / 2;
            }
            else
            {
                _targetOrthographicSize = (bounds.size.x / screenRatio) / 2;
            }
            _isTransitioning = true;
            
            Debug.Log($"Camera fitted to board. Bounds: {bounds.size}, Orthographic Size: {_targetOrthographicSize}");
        }
        
        // in case the board size changes
        [ContextMenu("Refit Camera")]
        public void RefitCamera()
        {
            FitToBoard();
        }
    }
}

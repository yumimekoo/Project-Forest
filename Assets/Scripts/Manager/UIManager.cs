using System;
using UnityEngine;
using UnityEngine.InputSystem;


public enum UIState { BuildMode, YarnOverlay, RecipeBook, Shop, Overlay, Pause, Tutorial }
    public class UIManager: MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private UIState currentState;
        public InputActionAsset InputActions;
        public InputAction openPause;
        public InputAction openBook;
        public event Action<UIState> OnUIStateChanged;
        public event Action OnButtonsUpdated;
        public event Action OnEscapePressed;
        public event Action OnPausePressed;
        public event Action OnRecipeBookPressed;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            openPause = InputSystem.actions.FindAction("OpenPause");
            openBook = InputSystem.actions.FindAction("BookInteract");
        }
        
        private void OnEnable()
        {
            InputActions.FindActionMap("Player").Enable();
        }

        private void OnDisable()
        {
            InputActions.FindActionMap("Player").Disable();
        }
        
        private void Start()
        {
            SetUIState(UIState.Overlay);
            Debug.Log("UI Started");
        }

        private void Update()
        {
            HandleInput();
        }
        
        public void SetUIState(UIState state)
        {
            if(state == currentState) return;
            currentState = state;
            OnUIStateChanged?.Invoke(state);
            Debug.Log($"UI State changed to {state}");
        }
        
        public void UpdateButtons()
        {
            OnButtonsUpdated?.Invoke();
        }

        public void ResetState()
        {
            SetUIState(UIState.Overlay);
            Debug.Log("Resetting UI State");
            Debug.LogWarning("Room:" + GameState.isInRoom + " Cafe:" + GameState.isInCafe);
        }

        public void RecipeBookButtonPressed()
        {
            OnRecipeBookPressed?.Invoke();
        }

        public void PauseButtonPressed()
        {
            if (!GameState.isInMenu)
            OnPausePressed?.Invoke();
        }

        private void HandleInput()
        {
            if (openPause.WasPressedThisFrame())
            {
                if (GameState.isInMenu)
                {
                    OnEscapePressed?.Invoke();    
                    Debug.Log("Escape Pressed");
                }
                else
                {
                    OnPausePressed?.Invoke();
                    Debug.Log("Pause Pressed");
                }
            }

            if (openBook.WasPressedThisFrame() && !GameState.isInRoom && !GameState.isInConversation && !GameState.isInPauseMenu && !GameState.isInStorage) OnRecipeBookPressed?.Invoke();
        }
        
        
    }

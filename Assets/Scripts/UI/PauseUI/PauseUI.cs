using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    public UIDocument pauseUI;
    public Sprite[] pauseSprites;

    public float animationFPS = 12f;
    
    private Button
        continueButton,
        yesButton,
        noButton,
        saveExitButton;
    private VisualElement 
        animationBackground,
        root,
        areYouSure;

    public InputActionAsset InputActions;
    public InputAction openPause;

    public enum PauseState { Opening, Opened, Closing, Closed }
    private PauseState state = PauseState.Closed;
    private float frameIndex = 0f;
    private float frameTimer = 0f;
    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
        UIManager.Instance.OnPausePressed += HandleInput;
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
        if(!UIManager.Instance) Debug.LogError("UIManager not found!");
        UIManager.Instance.OnPausePressed -= HandleInput;
    }

    private void Awake()
    {
        openPause = InputSystem.actions.FindAction("OpenPause");
    }

    private void Start()
    {
        root = pauseUI.rootVisualElement;
        continueButton = root.Q<Button>("continue");
        saveExitButton = root.Q<Button>("saveExit");
        animationBackground = root.Q<VisualElement>("animation");
        
        areYouSure = root.Q<VisualElement>("areYouSure");
        yesButton = root.Q<Button>("yesButton");
        noButton = root.Q<Button>("noButton");
        
        continueButton.clicked += RequestClose;
        saveExitButton.clicked += OnSaveExitButtonClicked;

        yesButton.clicked += OnConfirmYes;
        noButton.clicked += OnConfirmNo;

        HideConfirm();

        SetButtons(false);
        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        UpdateAnimation();
    }
    
    private void HandleInput()
    {
        if (IsConfirmVisible())
        {
            HideConfirm();
            return;
        }
        
        switch (state)
        {
            case PauseState.Closed:
                StartOpening();
                break;
            case PauseState.Opened:
                StartClosing();
                break;
            case PauseState.Opening:
                StartClosing();
                break;
            case PauseState.Closing:
                StartOpening();
                break;
            default:
                break;
        }
    }

    private void StartOpening()
    {
        ShowUI();
        SetButtons(false);
        state = PauseState.Opening;
    }

    private void StartClosing()
    {
        SetButtons(false);
        state = PauseState.Closing;
    }

    private void RequestClose()
    {
        if(state == PauseState.Opened)
            StartClosing();
    }

    private void FinishClosing()
    {
        state = PauseState.Closed;
        HideUI();
    }

    private void UpdateAnimation()
    {
        if (state != PauseState.Opening && state != PauseState.Closing)
            return;

        frameTimer += Time.unscaledDeltaTime;
        float frameDuration = 1f / animationFPS;
        if(frameTimer < frameDuration)
            return;

        frameTimer -= frameDuration;

        frameIndex += (state == PauseState.Opening) ? 1f : -1f;
        frameIndex = Mathf.Clamp(frameIndex, 0f, pauseSprites.Length - 1);
        animationBackground.style.backgroundImage = new StyleBackground(pauseSprites[Mathf.RoundToInt(frameIndex)]);

        if(state == PauseState.Opening && frameIndex >= pauseSprites.Length - 1)
        {
            state = PauseState.Opened;
            SetButtons(true);
        }
        else if(state == PauseState.Closing && frameIndex <= 0f)
        {
            FinishClosing();
        }
    }

    private void SetButtons(bool active)
    {
        continueButton.SetEnabled(active);
        saveExitButton.SetEnabled(active);
        continueButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        saveExitButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        //exitButton.SetEnabled(enabled);
    }
    private void OnSaveExitButtonClicked()
    {
        if (GameState.isInCafe && !GameState.isInRoom)
        {
            ShowConfirm();
            return;
        }
        
        GoToMenu(withSave: true);
    }

    private void OnConfirmYes()
    {
        HideConfirm();
        GoToMenu(withSave: false);
    }

    private void OnConfirmNo()
    {
        HideConfirm();
        SetButtons(state == PauseState.Opened);
    }

    private void GoToMenu(bool withSave)
    {
        Time.timeScale = 1f;
        
        if(withSave && SaveManager.Instance) SaveManager.Instance.SaveGame();
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    private void ShowConfirm()
    {
        SetButtons(false);
        areYouSure.style.display = DisplayStyle.Flex;
        yesButton?.SetEnabled(true);
        noButton?.SetEnabled(true);
    }
    
    private void HideConfirm()
    {
        areYouSure.style.display = DisplayStyle.None;
        yesButton?.SetEnabled(false);
        noButton?.SetEnabled(false);
        if(state == PauseState.Opened)
            SetButtons(true);
    }
    
    private bool IsConfirmVisible() => areYouSure.style.display == DisplayStyle.Flex;

    private void ShowUI()
    {
        pauseUI.rootVisualElement.style.display = DisplayStyle.Flex;
        UIManager.Instance.SetUIState(UIState.Pause);
        GameState.isInPauseMenu = true;
        Time.timeScale = 0f;
    }
    private void HideUI()
    {
        Time.timeScale = 1f;
        GameState.isInPauseMenu = false;
        UIManager.Instance.ResetState();
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
    }
}

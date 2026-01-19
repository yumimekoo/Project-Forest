using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    public UIDocument pauseUI;
    public Sprite[] pauseSprites;

    public float animationFPS = 12f;

    private bool isOpen = false;
    private Button
        continueButton,
       // exitButton,
        saveExitButton;
    private VisualElement animationBackground,
        root;

    public InputActionAsset InputActions;
    public InputAction openPause;

    public enum PauseState { Opening, Opened, Closing, Closed }
    private PauseState state = PauseState.Closed;
    private float frameIndex = 0f;
    private float frameTimer = 0f;
    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
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
        // exitButton = root.Q<Button>("exit");
        ////exitButton.clicked += ExitPlaymodeDevelopment;
        continueButton.clicked += RequestClose;
        saveExitButton.clicked += OnSaveExitButtonClicked;

        SetButtons(false);
        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {

        HandleInput();
        UpdateAnimation();

        //if(openPause.WasPressedThisFrame() && isOpen && GameState.playerInteractionAllowed)
        //{
        //    HideUI();
        //}
        //else if(openPause.WasPressedThisFrame() && !isOpen && GameState.playerInteractionAllowed)
        //{
        //    ShowUI();
        //}
    }

    private void HandleInput()
    {
        if(!openPause.WasPressedThisFrame()) return;

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
        }
    }

    private void StartOpening()
    {
        Time.timeScale = 0f;
        root.style.display = DisplayStyle.Flex;
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
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
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

    private void SetButtons(bool enabled)
    {
        continueButton.SetEnabled(enabled);
        saveExitButton.SetEnabled(enabled);
        //exitButton.SetEnabled(enabled);
    }
    private void OnSaveExitButtonClicked()
    {
        Time.timeScale = 1f;
        SaveManager.Instance.SaveGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    private void ExitPlaymodeDevelopment()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowUI()
    {
        pauseUI.rootVisualElement.style.display = DisplayStyle.Flex;
        isOpen = true;
    }
    public void HideUI()
    {
        pauseUI.rootVisualElement.style.display = DisplayStyle.None;
        isOpen = false;
    }
}

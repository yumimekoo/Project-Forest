using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    public UIDocument pauseUI;
    private bool isOpen = false;
    private Button
        continueButton,
        exitButton,
        saveExitButton;

    public InputActionAsset InputActions;
    public InputAction openPause;
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
        VisualElement root = pauseUI.rootVisualElement;
        continueButton = root.Q<Button>("continue");
        saveExitButton = root.Q<Button>("saveExit");
        exitButton = root.Q<Button>("exit");
        exitButton.clicked += ExitPlaymodeDevelopment;
        continueButton.clicked += HideUI;
        saveExitButton.clicked += OnSaveExitButtonClicked;

        HideUI();
    }

    private void Update()
    {
        if(openPause.WasPressedThisFrame() && isOpen)
        {
            HideUI();
        }
        else if(openPause.WasPressedThisFrame() && !isOpen)
        {
            ShowUI();
        }
    }

    private void OnSaveExitButtonClicked()
    {
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

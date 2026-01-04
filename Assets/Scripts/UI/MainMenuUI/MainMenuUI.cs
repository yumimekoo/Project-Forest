using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement root;
    private Button
        save1Button,
        save2Button,
        save3Button,
        exitButton;

    private void Awake()
    {
        root = uiDocument.rootVisualElement;
        save1Button = root.Q<Button>("save1");
        save2Button = root.Q<Button>("save2");
        save3Button = root.Q<Button>("save3");
        exitButton = root.Q<Button>("exit");

        save1Button.clicked += () => OnSaveButtonClicked(0);
        save2Button.clicked += () => OnSaveButtonClicked(1);
        save3Button.clicked += () => OnSaveButtonClicked(2);

        exitButton.clicked += OnExitButtonClicked;

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        UpdateSlotButton(save1Button, 0);
        UpdateSlotButton(save2Button, 1);
        UpdateSlotButton(save3Button, 2);
    }

    private void UpdateSlotButton(Button button, int slot)
    {
        SaveSlotInfo info = SaveSystem.LoadSlotInfo(slot);

        if (info == null || !info.exists)
        {
            button.text = $"Slot {slot + 1}\n<empty>";
        }
        else
        {
            button.text =
                $"{info.saveName}\n" +
                $"Created: {info.createdDate}\n" +
                $"Last Played: {info.lastModifiedDate}";
        }
    }

    private void OnSaveButtonClicked(int slot)
    {
        SaveManager.Instance.SetActiveSaveSlot(slot);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}



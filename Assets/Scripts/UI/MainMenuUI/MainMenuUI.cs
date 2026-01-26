using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI")]
    public UIDocument mainMenuUI;
    
    private VisualElement root;

    private VisualElement
        titleElement,
        playElement,
        areYouSure;

    private Button
        exitButton,
        savesScreenButton,
        backButton,
        deleteModeButton,
        save1Button,
        save2Button,
        save3Button,
        yesButton,
        noButton;
    
    private Label
        upperLabel1,
        upperLabel2,
        upperLabel3,
        lowerLabel1,
        lowerLabel2,
        lowerLabel3;

    private bool inDeletionMode = false;
    private int pendingDeleteSlot = -1;

    private const string SaveButtonClass = "saveButtons";
    private const string DeleteSelectedClass = "deleteSelected";

    private void Awake()
    {
       root = mainMenuUI.rootVisualElement;
       
       titleElement = root.Q<VisualElement>("titleElement");
       playElement = root.Q<VisualElement>("playElement");
       areYouSure = root.Q<VisualElement>("areYouSure");
       
       exitButton = root.Q<Button>("exit");
       savesScreenButton = root.Q<Button>("savesScreen");
       backButton = root.Q<Button>("backButton");
       deleteModeButton = root.Q<Button>("deleteMode");
       
       save1Button = root.Q<Button>("save1");
       save2Button = root.Q<Button>("save2");
       save3Button = root.Q<Button>("save3");
       
       upperLabel1 = root.Q<Label>("upperLabel1");
       upperLabel2 = root.Q<Label>("upperLabel2");
       upperLabel3 = root.Q<Label>("upperLabel3");
       lowerLabel1 = root.Q<Label>("lowerLabel1");
       lowerLabel2 = root.Q<Label>("lowerLabel2");
       lowerLabel3 = root.Q<Label>("lowerLabel3");
       
       yesButton = root.Q<Button>("yesButton");
       noButton = root.Q<Button>("noButton");

       exitButton.clicked += OnExitClicked;
       savesScreenButton.clicked += ShowPlayScreen;

       backButton.clicked += () =>
       {
           SetDeletionMode(false);
           HideAreYouSure();
           ShowTitleScreen();
       };
       
       deleteModeButton.clicked += ToggleDeletionMode;
       
       save1Button.clicked += () => OnSaveSlotClicked(0);
       save2Button.clicked += () => OnSaveSlotClicked(1);
       save3Button.clicked += () => OnSaveSlotClicked(2);
       
       noButton.clicked += () =>
       {
           pendingDeleteSlot = -1;
           HideAreYouSure();
       };

       yesButton.clicked += () =>
       {
           if (pendingDeleteSlot < 0) return;

           SaveSystem.DeleteSave(pendingDeleteSlot);

           pendingDeleteSlot = -1;
           ToggleDeletionMode();
           HideAreYouSure();
           UpdateSlotsUI();
       };
       
       ShowTitleScreen();
       SetDeletionMode(false);
       HideAreYouSure();
       UpdateSlotsUI();
    }

    private void ShowPlayScreen()
    {
        titleElement.style.display = DisplayStyle.None;
        playElement.style.display = DisplayStyle.Flex;
        SetDeletionMode(false);
        HideAreYouSure();
        UpdateSlotsUI();
    }

    private void ShowTitleScreen()
    {
        titleElement.style.display = DisplayStyle.Flex;
        playElement.style.display = DisplayStyle.None;
        SetDeletionMode(false);
    }

    private void UpdateSlotsUI()
    {
        UpdateSingleSlotUI(0, upperLabel1, lowerLabel1);
        UpdateSingleSlotUI(1, upperLabel2, lowerLabel2);
        UpdateSingleSlotUI(2, upperLabel3, lowerLabel3);
    }

    private void UpdateSingleSlotUI(int slot, Label upper, Label lower)
    {
        SaveSlotInfo info = SaveSystem.LoadSlotInfo(slot);

        if (info == null || !info.exists)
        {
            upper.text = "New Save";
            lower.text = "Click on play to Create";
            return;
        } 
        
        string created = FormatDate(info.createdDate, false);
        string last = FormatDate(info.lastModifiedDate, true);
        
        upper.text = $"{info.currentWeekday} {info.currentDay}, Week {info.currentWeek} || {info.currentMoney}";
        lower.text = $"Created: {created} || Last: {last}";
    }

    private void ToggleDeletionMode()
    {
        SetDeletionMode(!inDeletionMode);
        if (!inDeletionMode)
        {
            pendingDeleteSlot = -1;
            HideAreYouSure();
        }
    }

    private void SetDeletionMode(bool on)
    {
        inDeletionMode = on;
        
        ApplyDeletionClass(save1Button, on);
        ApplyDeletionClass(save2Button, on);
        ApplyDeletionClass(save3Button, on);
    }

    private void ApplyDeletionClass(Button button, bool on)
    {
        if (on)
        {
            button.RemoveFromClassList(SaveButtonClass);
            button.AddToClassList(DeleteSelectedClass);
        }
        else
        {
            button.RemoveFromClassList(DeleteSelectedClass);
            button.AddToClassList(SaveButtonClass);
        }
    }
    
    private void OnSaveSlotClicked(int slot)
    {
        if (inDeletionMode)
        {
            pendingDeleteSlot = slot;
            ShowAreYouSure();
            return;
        }
        
        SetDeletionMode(false);
        HideAreYouSure();
        
        SaveManager.Instance.SetActiveSaveSlot(slot);
        SceneManager.LoadScene("Room");
    }

    private void ShowAreYouSure()
    {
        if (areYouSure == null) return;
        areYouSure.style.display = DisplayStyle.Flex;
        areYouSure.SetEnabled(true);
    }
    private void HideAreYouSure()
    {
        if (areYouSure == null) return;
        areYouSure.style.display = DisplayStyle.None;
        areYouSure.SetEnabled(false);
    }
    
    private string FormatDate(string raw, bool withTime)
    {
        if (!System.DateTime.TryParse(raw, out var dt))
            return raw;

        return withTime
            ? dt.ToString("dd.MM.yy HH:mm")
            : dt.ToString("dd.MM.yy");
    }
    
    private void OnExitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}



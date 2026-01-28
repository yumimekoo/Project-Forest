using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI")]
    public UIDocument mainMenuUI;

    [Header("Loading Animation")] 
    public Sprite[] loadingFrames;
    public float loadingFps = 12f;
    
    [Header("Music and Sound")]
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip roomMusic;
    [SerializeField] private SoundSO hoverSound;
    [SerializeField] private SoundSO clickSound;
    
    private VisualElement root;

    private VisualElement
        titleElement,
        playElement,
        areYouSure,
        loadingOverlay,
        loadingImage;

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
    private Coroutine loadingAnimRoutine;

    private const string SaveButtonClass = "saveButtons";
    private const string DeleteSelectedClass = "deleteSelected";

    private void Awake()
    {
       root = mainMenuUI.rootVisualElement;
       
       titleElement = root.Q<VisualElement>("titleElement");
       playElement = root.Q<VisualElement>("playElement");
       areYouSure = root.Q<VisualElement>("areYouSure");
       
       loadingOverlay = root.Q<VisualElement>("loadingOverlay");
       loadingImage = root.Q<VisualElement>("loadingImage");
       HideLoadingInstant();
       
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
       
       foreach (var button in root.Query<Button>().ToList())
       {
           button.RegisterCallback<MouseEnterEvent>(_ =>
           {
               AudioManager.Instance.Play(hoverSound);
           });

           button.clicked += () =>
           {
               AudioManager.Instance.Play(clickSound);
           };
       }
       
       ShowTitleScreen();
       SetDeletionMode(false);
       HideAreYouSure();
       UpdateSlotsUI();
    }

    private void Start()
    {
       if(AudioManager.Instance) AudioManager.Instance.CrossfadeMusic(mainMusic, fadeSeconds: 2f, syncWithCurrent: false);
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
        StartCoroutine(LoadRoomRoutine());
    }

    private IEnumerator LoadRoomRoutine()
    {
        ShowLoading();
        yield return null;
        
        float minTime = 1f;
        float start = Time.realtimeSinceStartup;
        
        var op = SceneManager.LoadSceneAsync("Room");
        op.allowSceneActivation = false;
        
        float elapsed = Time.realtimeSinceStartup - start;
        if (elapsed < minTime)
            yield return new WaitForSecondsRealtime(minTime - elapsed);
        
        while (op.progress < 0.9f) yield return null;
        
        yield return new WaitForSeconds(0.15f);
        
        AudioManager.Instance.CrossfadeMusic(roomMusic, fadeSeconds: 2f, syncWithCurrent: false);
        
        op.allowSceneActivation = true;
        
        while(!op.isDone) yield return null;
    
        HideLoading();
    }

    private void ShowLoading()
    {
        loadingOverlay.style.display = DisplayStyle.Flex;
        loadingOverlay.SetEnabled(true);
        loadingOverlay.pickingMode = PickingMode.Position;

        loadingOverlay.style.opacity = 0f;
        
        if(loadingAnimRoutine != null) StopCoroutine(loadingAnimRoutine);
        loadingAnimRoutine = StartCoroutine(PlayLoadingPingPong());
        
        StartCoroutine(SetOpacityNextFrame(1f));
    }

    private void HideLoading()
    {
        loadingOverlay.style.opacity = 0f;
        loadingOverlay.pickingMode = PickingMode.Ignore;
        
        if(loadingAnimRoutine != null) StopCoroutine(loadingAnimRoutine);
        loadingAnimRoutine = null;

        StartCoroutine(HideAfterSeconds(0.25f));
    }
    
    private IEnumerator SetOpacityNextFrame(float opacity)
    {
        yield return null;
        loadingOverlay.style.opacity = opacity;
    }
    
    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideLoadingInstant();
    }

    private void HideLoadingInstant()
    {
        loadingOverlay.style.display = DisplayStyle.None;
        loadingOverlay.SetEnabled(false);
    }

    private IEnumerator PlayLoadingPingPong()
    {
        if (loadingFrames == null || loadingFrames.Length == 0) yield break;

        int i = 0;
        int dir = 1;
        float delay = 1f / Mathf.Max(1f, loadingFps);
        
        while(true)
        {
            loadingImage.style.backgroundImage = new StyleBackground(loadingFrames[i]);
            if (loadingFrames.Length > 1)
            {
                if(i == loadingFrames.Length - 1) dir = -1;
                else if (i == 0) dir = 1;
                i += dir;
            }
            yield return new WaitForSeconds(delay);
        }
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



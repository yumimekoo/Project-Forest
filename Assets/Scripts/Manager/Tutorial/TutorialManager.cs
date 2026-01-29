using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum TutorialStep
{
    StartMonologue,
    PressEOnBuilder,
    BuilderMonologue,

    PressDeleteMode,  
    DeleteAllObjects,
    PlaceMonologue,
    PlaceAllObjects,
    ExitBuildMode,

    BuyMonologue,
    PressEOnShop,
    BuyItem,
    ExitShop,

    DoorMonologue,
    PressEOnDoor,

    CafeIntroMonologue, 
    WaitingForNPCSpawn,
    FirstNPCSpawned,
    NPCMonologue,
    TakeOrder,
    RecipeBookMonologue,
    OpenRecipeBook,
    CloseRecipeBook,
    MakeCoffeeMonologue,
    MakeCoffee,
    
    GiveCoffee,
    EndMonologue,
    TutorialDone
}
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TutorialStep currentStep;

    private int objectsDeleted = 0;
    private int objectsPlaced = 0;
    private int objectBought = 0;

    private Label
        titleLabel,
        descriptionLabel;

    private VisualElement root;

    private Coroutine uiAnimation;

    [Header("Presets")]
    [SerializeField] private int objectsToDeleteAndPlace = 5; // check inspector
    [SerializeField] private int itemsToBuy = 10; // check inspector

    [Header("References")]
    [SerializeField] YarnManagerTutorial yarnManagerTutorial;
    [SerializeField] UIDocument tutorialUI;
    [SerializeField] SoundSO showSound;
    [SerializeField] SoundSO hideSound;
    private GameObject builderGlow;
    private GameObject shopGlow;
    private GameObject doorGlow;
    private GameObject npcGlow;
    private GameObject coffeeMachineGlow;
    private GameObject drawerGlow;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;    
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReloadReferences();
    }

    private void ReloadReferences()
    {
        builderGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("BuilderGlow"));
        shopGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("ShopGlow"));
        doorGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("DoorGlow"));
        npcGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("NPCGlow"));
        coffeeMachineGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("CoffeeMachineGlow"));
        drawerGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("DrawerGlow"));

        HandleHighlight(TutorialStep.StartMonologue);

    }

    private void ReloadNPCGlow()
    {
        npcGlow = Resources.FindObjectsOfTypeAll<GameObject>()
        .FirstOrDefault(go => go.CompareTag("NPCGlow"));
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!GameState.inTutorial)
        {
            Destroy(gameObject);
            return;
        }

        root = tutorialUI.rootVisualElement;
        titleLabel = root.Q<Label>("titleLabel");
        descriptionLabel = root.Q<Label>("descriptionLabel");

        root.AddToClassList("tutorial-root");
        root.AddToClassList("tutorial-hidden");

        root.style.display = DisplayStyle.None;

        //Debug.Log("Tutorial Manager Initialized");
    }

    private void SetStep(TutorialStep step)
    {
        //Debug.LogWarning("Setting Tutorial Step to: " + step);
        currentStep = step;
        //Debug.Log($"Tutorial Step set to: {step}");
        HandleHighlight(step);
        HandleText(step);
        HandleUI(step);
        HandleGameStates(step);
    }

    public void CheckDestroyment()
    {

        if (!GameState.inTutorial && Instance != null)
        {
            UIManager.Instance.UpdateButtons();
            Destroy(gameObject);
            return;
        }
        if(GameState.isInRoom)
            SetStep(TutorialStep.StartMonologue);
    }

    private void Advance()
    {
        if(currentStep == TutorialStep.TutorialDone)
            return;
        SetStep(currentStep + 1);
    }

    private void AdvanceToEnd()
    {
        GameState.inTutorial = false;
        SetStep(TutorialStep.TutorialDone);
        //Debug.Log("Tutorial Completed!");
        TimeManager.Instance.TutorialComplete();
        Destroy(gameObject);
    }
    public void OnBuilderUsed()
    {
        if(currentStep == TutorialStep.PressEOnBuilder) 
            Advance();
    }

    public void OnDeletionModePressed()
    {
        if(currentStep == TutorialStep.PressDeleteMode) 
            Advance();
    }

    public void OnObjectDeleted()
    {
        if(currentStep == TutorialStep.DeleteAllObjects) 
        {
            objectsDeleted++;
            descriptionLabel.text = $"Delete {objectsToDeleteAndPlace - objectsDeleted} more objects.";
            if (objectsDeleted >= objectsToDeleteAndPlace)
            {
                Advance();
            }
        }
    }

    public void OnObjectPlaced()
    {
        if(currentStep == TutorialStep.PlaceAllObjects) 
        {
            objectsPlaced++;
            descriptionLabel.text = $"Place {objectsToDeleteAndPlace - objectsPlaced} more objects.";
            if (objectsPlaced >= objectsToDeleteAndPlace)
            {
                Advance();
                UIManager.Instance.UpdateButtons();
            }
        }
    }

    public void OnExitPressed()
    {
        if(currentStep == TutorialStep.ExitBuildMode || 
           currentStep == TutorialStep.ExitShop) 
        {
            Advance();
        }
    }

    public void OnShopUsed()
    {
        if(currentStep == TutorialStep.PressEOnShop) 
            Advance();
    }

    public void OnItemBought(int amount)
    {
        if(currentStep == TutorialStep.BuyItem) 
        {
            objectBought += amount;
            if(objectBought >= itemsToBuy)
            {
                Advance();
                UIManager.Instance.UpdateButtons();
            }
        }
    }

    public void OnDoorUsed()
    {
        if(currentStep == TutorialStep.PressEOnDoor) 
            Advance();
    }

    public void OnNPCSpawned() {
        if (currentStep == TutorialStep.WaitingForNPCSpawn)
        {
            Advance();
            ReloadNPCGlow();
        }
    }
    public void OnNPCSatDown() { 
        if(currentStep == TutorialStep.FirstNPCSpawned) 
            Advance();
    }
    public void OnOrderTaken() { 
        if(currentStep == TutorialStep.TakeOrder)
        {
            GameState.playerMovementAllowed = false;
            GameState.playerInteractionAllowed = false;
            Advance();
        }
    }
    public void OnRecipeBookOpened() { 
        if(currentStep == TutorialStep.OpenRecipeBook) 
            Advance();
    }
    public void OnRecipeBookClosed() { 
        if(currentStep == TutorialStep.CloseRecipeBook)
            Advance();
    }
    public void OnCoffeeMade() { 
        if(currentStep == TutorialStep.MakeCoffee) 
            Advance();
    }
    public void OnCoffeeGiven() { 
        if(currentStep == TutorialStep.GiveCoffee) 
            Advance();
    }

    public void OnDialogueFinished()
    {
        if(currentStep == TutorialStep.StartMonologue || 
           currentStep == TutorialStep.BuilderMonologue ||  
           currentStep == TutorialStep.BuyMonologue || 
           currentStep == TutorialStep.DoorMonologue ||
           currentStep == TutorialStep.CafeIntroMonologue ||
           currentStep == TutorialStep.NPCMonologue ||
           currentStep == TutorialStep.RecipeBookMonologue ||
           currentStep == TutorialStep.MakeCoffeeMonologue)

        {
            Advance();
        }

        if(currentStep == TutorialStep.PlaceMonologue)
        {
            Advance();
            UIManager.Instance.UpdateButtons();
        }

        if (currentStep == TutorialStep.EndMonologue)
        {
            AdvanceToEnd();
        }
    }

    private void HandleGameStates(TutorialStep step)
    {
        if (step != TutorialStep.CloseRecipeBook && step != TutorialStep.OpenRecipeBook) return;
        GameState.playerMovementAllowed = false;
        GameState.playerInteractionAllowed = false;

    }

    private void HandleHighlight(TutorialStep step)
    {
        //Debug.Log("Handling Highlight for step: " + step);
        if (builderGlow)
            builderGlow.SetActive(step == TutorialStep.PressEOnBuilder);
        if (shopGlow)
            shopGlow.SetActive(step == TutorialStep.PressEOnShop);
        if (doorGlow)
            doorGlow.SetActive(step == TutorialStep.PressEOnDoor);
        if (npcGlow)
            npcGlow.SetActive(step == TutorialStep.TakeOrder || step == TutorialStep.GiveCoffee);
        if (coffeeMachineGlow)
            coffeeMachineGlow.SetActive(step == TutorialStep.MakeCoffee);
        if (drawerGlow)
            drawerGlow.SetActive(step == TutorialStep.MakeCoffee);
    }

    private void HandleText(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.StartMonologue:
                yarnManagerTutorial.StartDialogue("StartMonologue");
                break;
            case TutorialStep.BuilderMonologue:
                yarnManagerTutorial.StartDialogue("BuilderMonologue");
                break;
            case TutorialStep.PlaceMonologue:
                yarnManagerTutorial.StartDialogue("PlaceMonologue");
                break;
            case TutorialStep.BuyMonologue:
                yarnManagerTutorial.StartDialogue("BuyMonologue");
                break;
            case TutorialStep.DoorMonologue:
                yarnManagerTutorial.StartDialogue("DoorMonologue");
                break;
            case TutorialStep.CafeIntroMonologue:
                yarnManagerTutorial.StartDialogue("CafeIntroMonologue");
                break;
            case TutorialStep.NPCMonologue:
                yarnManagerTutorial.StartDialogue("NPCMonologue");
                break;
            case TutorialStep.RecipeBookMonologue:
                yarnManagerTutorial.StartDialogue("RecipeBookMonologue");
                break;
            case TutorialStep.MakeCoffeeMonologue:
                yarnManagerTutorial.StartDialogue("MakeCoffeeMonologue");
                break;
            case TutorialStep.EndMonologue:
                yarnManagerTutorial.StartDialogue("EndMonologue");
                break;
            default:
                //Debug.Log("No dialogue for this step:" + step);
                break;
        }
    }

    private void HandleUI(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.PressEOnBuilder:
                titleLabel.text = "Open Builder.";
                descriptionLabel.text = "Walk up to the Builder and press E to open build mode.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.BuilderMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.PressDeleteMode:
                UIManager.Instance.SetUIState(UIState.BuildMode);
                titleLabel.text = "Remove Objects";
                descriptionLabel.text = "Press X to enter Remove Mode, then Left Click objects to remove them. Remove "+ objectsToDeleteAndPlace +" more objects to continue.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.PlaceMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.PlaceAllObjects:
                UIManager.Instance.SetUIState(UIState.BuildMode);
                titleLabel.text = "Place Furniture";
                descriptionLabel.text = "Press 'All' to select a build category." +
                    "Select furniture and place " + objectsToDeleteAndPlace+ " objects in your caf�.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.ExitBuildMode:
                StartUIRoutine(SwapRoutine("Exit Build Mode", "When you�re done building, press Esc or click the Exit button."));
                break;
            case TutorialStep.BuyMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.PressEOnShop:
                titleLabel.text = "Open Shop";
                descriptionLabel.text = "Walk up to the Shop and press E to browse and buy items.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.BuyItem:
                StartUIRoutine(SwapRoutine("Buy Items", "Buy "+itemsToBuy+" Coffee Beans from the Shop to prepare for making coffee."));
                break;
            case TutorialStep.ExitShop:
                StartUIRoutine(SwapRoutine("Exit Shop", "Click the Exit button to leave the Shop"));
                break;
            case TutorialStep.DoorMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.PressEOnDoor:
                titleLabel.text = "Enter Caf�";
                descriptionLabel.text = "Press E at the Door to enter your caf� and start the day.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.CafeIntroMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.FirstNPCSpawned:
                StartUIRoutine(ShowUIAnimated());
                titleLabel.text = "A Customer Arrived";
                descriptionLabel.text = "Wait a moment until the customer finds a seat and sits down.";
                break;
            case TutorialStep.NPCMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.TakeOrder:
                titleLabel.text = "Take Order";
                descriptionLabel.text = "Walk up to the customer and press E to take their order.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.RecipeBookMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.OpenRecipeBook:
                titleLabel.text = "Open Recipe Book";
                descriptionLabel.text = "Press TAB to open the Recipe Book and check active orders.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.CloseRecipeBook:
                StartUIRoutine(SwapRoutine("Close Recipe Book", "Press TAB again to close the Recipe Book and continue playing."));
                break;
            case TutorialStep.MakeCoffeeMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
            case TutorialStep.MakeCoffee:
                titleLabel.text = "Make Coffee";
                descriptionLabel.text = "Use the Coffee Machine to prepare the ordered drink. Add a mug and the required ingredients.";
                StartUIRoutine(ShowUIAnimated());
                break;
            case TutorialStep.GiveCoffee:
                StartUIRoutine(SwapRoutine("Serve Customer", "Bring the finished coffee to the customer and give it to them."));
                break;
            case TutorialStep.EndMonologue:
                StartUIRoutine(HideUIAnimated());
                titleLabel.text = "";
                descriptionLabel.text = "";
                break;
        }
    }

    private void StartUIRoutine(IEnumerator routine)
    {
        if (uiAnimation != null)
            StopCoroutine(uiAnimation);

        uiAnimation = StartCoroutine(routine);
    }
    
    private IEnumerator WaitForTransition(VisualElement element)
    {
        bool finished = false;

        void OnTransitionEnd(TransitionEndEvent evt)
        {
            finished = true;
        }

        element.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);

        while (!finished)
            yield return null;

        element.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
    }

    private IEnumerator HideUIAnimated()
    {
        AudioManager.Instance.Play(hideSound);
        root.RemoveFromClassList("tutorial-visible");
        root.AddToClassList("tutorial-hidden");

        yield return WaitForTransition(root);

        root.style.display = DisplayStyle.None;
    }

    private IEnumerator ShowUIAnimated()
    {
        AudioManager.Instance.Play(showSound);
        root.style.display = DisplayStyle.Flex;

        // 1 Frame warten, sonst triggert Transition nicht
        yield return null;

        root.RemoveFromClassList("tutorial-hidden");
        root.AddToClassList("tutorial-visible");

        yield return WaitForTransition(root);
    }

    private IEnumerator SwapRoutine(string title, string description)
    {
        yield return HideUIAnimated();

        titleLabel.text = title;
        descriptionLabel.text = description;

        yield return ShowUIAnimated();
    }
}

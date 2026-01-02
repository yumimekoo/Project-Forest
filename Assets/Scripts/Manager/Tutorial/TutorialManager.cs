using UnityEngine;

public enum TutorialStep
{
    StartMonologue, // add UI: text to go to builder
    PressEOnBuilder,
    BuilderMonologue, // add text to press Deletebutton and then LeftClick to delete all object

    PressDeleteMode,  
    DeleteAllObjects,
    PlaceMonologue, // change text to press delete button again to exit build mode and place all objects
    PlaceFiveObjects, 
    ExitBuildMode,  // press exit button to exit build mode, make Exit button visible now

    PressEOnShop,
    BuyMonologue, // add text to buy coffe
    BuyItem,     // add text to exit shop (button enables)
    ExitShop,

    DoorMonologue, // add text to go to door and press E
    PressEOnDoor,

    CafeIntro, 
    FirstNPCSpawned,
    TakeOrder,
    OpenRecipeBook,
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
    [SerializeField] YarnManagerTutorial yarnManagerTutorial;
    [SerializeField] GameObject builderGlow;
    [SerializeField] GameObject shopGlow;
    [SerializeField] GameObject doorGlow;

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
        Debug.Log("Tutorial Manager Initialized");
        SetStep(TutorialStep.StartMonologue);
    }

    private void SetStep(TutorialStep step)
    {
        Debug.LogWarning("Setting Tutorial Step to: " + step);
        currentStep = step;
        Debug.Log($"Tutorial Step set to: {step}");
        HandleHighlight(step);
        HandleText(step);
    }

    private void Advance()
    {
        if(currentStep == TutorialStep.TutorialDone)
            return;
        SetStep(currentStep + 1);
    }

    // -- Events --

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

    public void OnDialogueFinished()
    {
        if(currentStep == TutorialStep.StartMonologue || 
           currentStep == TutorialStep.BuilderMonologue || 
           currentStep == TutorialStep.PlaceMonologue || 
           currentStep == TutorialStep.BuyMonologue || 
           currentStep == TutorialStep.DoorMonologue || 
           currentStep == TutorialStep.EndMonologue)
        {
            Advance();
        }
    }

    void HandleHighlight(TutorialStep step)
    {
        Debug.Log("Handling Highlight for step: " + step);
        builderGlow.SetActive(step == TutorialStep.PressEOnBuilder);
        shopGlow.SetActive(step == TutorialStep.PressEOnShop);
        doorGlow.SetActive(step == TutorialStep.PressEOnDoor);
    }

    void HandleText(TutorialStep step)
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
                yarnManagerTutorial.StartDialogue("StartMonologue");
                break;
            case TutorialStep.BuyMonologue:
                yarnManagerTutorial.StartDialogue("StartMonologue");
                break;
            case TutorialStep.DoorMonologue:
                yarnManagerTutorial.StartDialogue("StartMonologue");
                break;
            case TutorialStep.EndMonologue:
                yarnManagerTutorial.StartDialogue("StartMonologue");
                break;
            default:
                Debug.Log("No dialogue for this step: " + step);
                break;
        }
    }
}

using UnityEngine;
using Yarn.Unity;

public class YarnManagerTutorial : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public GameObject dialogueObject;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueFinished);
    }

    private void OnDisable()
    {
        dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueFinished);
    }

    void OnDialogueFinished()
    {
        UIManager.Instance.ResetState();
        dialogueObject.SetActive(false);
        //Debug.Log("[Yarn] Dialogue finished");
        GameTime.SetPaused(false);
        GameState.playerMovementAllowed = true;
        GameState.playerInteractionAllowed = true;
        TutorialManager.Instance.OnDialogueFinished();
        
    }

    public void StartDialogue(string startNode)
    {
        dialogueObject.SetActive(true);
        dialogueRunner.StartDialogue(startNode);
        GameState.playerMovementAllowed = false;
        GameState.playerInteractionAllowed = false;
        UIManager.Instance.SetUIState(UIState.YarnOverlay);
    }

    public void StopDialogue()
    {
        dialogueObject.SetActive(false);
        dialogueRunner.Stop();
    }
}


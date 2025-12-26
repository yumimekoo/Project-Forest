using UnityEngine;
using Yarn.Unity;

public class YarnManager : MonoBehaviour
{
    public static YarnManager Instance;

    public DialogueRunner dialogueRunner;
    public GameObject dialogueObject;

    private void Awake()
    {
        Instance = this;
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
        dialogueObject.SetActive(false);
        Debug.Log("[Yarn] Dialogue finished");
        GameTime.SetPaused(false);
        //GameState.playerMovementAllowed = true;
    }

    public void StartDialogue(YarnProject project, string startNode)
    {
        dialogueObject.SetActive(true);
        dialogueRunner.SetProject(project);
        dialogueRunner.StartDialogue(startNode);
    }

    public void StopDialogue()
    {
        dialogueObject.SetActive(false);
        dialogueRunner.Stop();
    }
}

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
        GameTime.SetPaused(false);
        UIManager.Instance.ResetState();
    }

    public void StartDialogue(YarnProject project, string startNode)
    {
        GameTime.SetPaused(true);
        dialogueObject.SetActive(true);
        dialogueRunner.SetProject(project);
        dialogueRunner.StartDialogue(startNode);
        UIManager.Instance.SetUIState(UIState.YarnOverlay);
    }

    public void StopDialogue()
    {
        dialogueObject.SetActive(false);
        dialogueRunner.Stop();
    }
}

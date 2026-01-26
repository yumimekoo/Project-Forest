using UnityEngine;
using UnityEngine.UIElements;

public class ShopBuilder : MonoBehaviour, IInteractable
{
    private CameraFollow cameraFollow;

    public void Start()
    {
        if (cameraFollow == null)
        {
            cameraFollow = FindAnyObjectByType<CameraFollow>();
        }
    }
    public string GetInteractionPrompt()
    {
        return "Open Shop-Editor";
    }

    public void Interact(PlayerInventory player)
    {
        UIManager.Instance.SetUIState(UIState.BuildMode);
        if(GameState.inTutorial)
        {
            TutorialManager.Instance.OnBuilderUsed();
        }

        GameState.playerInteractionAllowed = false;
        GameState.isInBuildMode = true;
        cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);
    }
}

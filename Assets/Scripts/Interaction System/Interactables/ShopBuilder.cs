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
        GameState.playerInteractionAllowed = false;
        GameState.isInBuildMode = true;

        BuildModeUI.Instance.ShowUI();
        cameraFollow.ChangeFollowTarget(GameState.isInBuildMode);
    }
}

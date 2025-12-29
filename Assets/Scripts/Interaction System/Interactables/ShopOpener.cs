using UnityEngine;
using UnityEngine.UIElements;

public class ShopOpener : MonoBehaviour, IInteractable
{
    private CameraFollow cameraFollow;
    public string GetInteractionPrompt()
    {
        return "Open Shop and buy something";
    }

    public void Interact(PlayerInventory player)
    {
        ShopUI.Instance.ShowUI();
    }
}

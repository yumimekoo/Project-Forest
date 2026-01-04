using UnityEngine;
using UnityEngine.UIElements;

public class ShopOpener : MonoBehaviour, IInteractable
{
    private CameraFollow cameraFollow;
    public string GetInteractionPrompt()
    {
        if(TutorialManager.Instance != null && GameState.inTutorial)
        {
            if (TutorialManager.Instance.currentStep < TutorialStep.PressEOnShop)
                return "Not yet.. IS THIS GLOWING OR WHAT??";
        }

        return "Open Shop and buy something";
    }

    public void Interact(PlayerInventory player)
    {
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep < TutorialStep.PressEOnShop)
                return;

                TutorialManager.Instance.OnShopUsed();
        }

        ShopUI.Instance.ShowUI();
    }
}

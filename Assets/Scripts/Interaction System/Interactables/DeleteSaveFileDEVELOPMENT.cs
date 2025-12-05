using UnityEngine;

public class DeleteSaveFileDEVELOPMENT : MonoBehaviour, IInteractable
{
    public string GetInteractionPrompt()
    {
        return "Delete Save File (Development Only)";
    }

    public void Interact(PlayerInventory player)
    {
        SaveSystem.DeleteSave(SaveManager.Instance.ActiveSaveSlot);

    }
}

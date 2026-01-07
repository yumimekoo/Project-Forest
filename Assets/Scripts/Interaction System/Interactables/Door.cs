using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string GetInteractionPrompt()
    {
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep < TutorialStep.PressEOnDoor)
                return "The door is also not the GLOWING object";
        }

        if (GameState.isInRoom)
        {
            return "Exit Room";
        }
        else
        {
            return "Enter Room";
        }
    }

    public void Interact(PlayerInventory player)
    {
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep < TutorialStep.PressEOnDoor)
                return;
            TutorialManager.Instance.OnDoorUsed();
        }

        if (player.HasItem())
        {
            player.ClearItem();
            ChangeScene();
        }
        else
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        if (FurnitureInventory.Instance.GetOccupiedCellsCount() < 5) // chnage that here please
        {
            //Debug.Log("You need to have at least 5 pieces of furniture placed to enter the room.");
            return;
        }
           

        if (GameState.isInRoom)
        {
            GameState.isInRoom = false;
            GameState.isInCafe = true;
            GameState.doorUnlocked = false;
            //Debug.Log("Player has exited the room to the cafe.");
            SaveManager.Instance.SaveGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
        else if (GameState.isInCafe && GameState.doorUnlocked)
        {
            GameState.isInRoom = true;
            GameState.isInCafe = false;
            //Debug.Log("Player has entered the room from the cafe.");
            SaveManager.Instance.SaveGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
        } else if (!GameState.doorUnlocked)
        { 
            //Debug.Log("The door is locked.");
        } else 
        {
            Debug.LogWarning("Player is not in a recognized location.");
        }

    }
}

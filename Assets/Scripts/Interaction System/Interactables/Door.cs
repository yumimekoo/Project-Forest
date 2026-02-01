using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip cafeDayMusic;
    [SerializeField] private AudioClip roomMusic;
    [SerializeField] private SoundSO doorSound;
    [SerializeField] private SoundSO doorLockedSound;
    public string GetInteractionPrompt()
    {
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep < TutorialStep.PressEOnDoor)
                return "The door is also not the GLOWING object";
        }

        return GameState.isInRoom ? null : "Enter Room";
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
            SaveManager.Instance.SaveGame();
            AudioManager.Instance.CrossfadeMusic(cafeDayMusic, 3f);
            AudioManager.Instance.Play(doorSound);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
            if(GameState.inTutorial && TutorialManager.Instance)
                TutorialManager.Instance.ReloadReferences();
        }
        else if (GameState.isInCafe && GameState.doorUnlocked)
        {
            GameState.isInRoom = true;
            GameState.isInCafe = false;
            SaveManager.Instance.SaveGame();
            AudioManager.Instance.CrossfadeMusic(roomMusic, 3f);
            AudioManager.Instance.Play(doorSound);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
        } else if (!GameState.doorUnlocked)
        { 
            AudioManager.Instance.Play(doorLockedSound);
        } else 
        {
            Debug.LogWarning("Player is not in a recognized location.");
        }

    }
}

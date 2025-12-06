using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private void Start()
    {
        SaveManager.Instance.LoadGame();
    }
}


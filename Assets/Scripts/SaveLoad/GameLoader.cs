using UnityEngine;
using UnityEngine.EventSystems;

public class GameLoader : MonoBehaviour
{
    private void Start()
    {
        SaveManager.Instance.LoadGame();
    }
}


using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance;

    [SerializeField] private UnlockDatabaseSO unlockDatabaseTemplate;
    public UnlockDatabaseSO runtimeDatabase { get; private set; }

    private void Awake()
    {
        Instance = this;
        runtimeDatabase = ScriptableObject.Instantiate(unlockDatabaseTemplate);
    }
}

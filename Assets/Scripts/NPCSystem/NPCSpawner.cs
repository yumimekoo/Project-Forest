using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public NPCIdentitySO npcToSpawn;
    // pooling later on
    public void Spawn()
    {
        Instantiate(npcToSpawn.npcPrefab, transform.position, transform.rotation);
    }
}

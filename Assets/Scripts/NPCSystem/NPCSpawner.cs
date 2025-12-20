using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public NPCIdentitySO npcToSpawn;

    private float spawnInterval = 5f;
    private float timeSinceLastSpawn = 0f;

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && ChairManager.Instance.GetFreeChair() != null)
        {
            Spawn();
            timeSinceLastSpawn = 0f;
        }
    }
    // pooling later on
    public void Spawn()
    {
        Instantiate(npcToSpawn.npcPrefab, transform.position, transform.rotation);
    }
}

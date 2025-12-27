using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    private float spawnInterval = 5f;
    private float timeSinceLastSpawn = 0f;

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && ChairManager.Instance.GetFreeChair() != null && NPCPool.Instance.HasNPCs)
        {
            Spawn();
            timeSinceLastSpawn = 0f;
        }
    }
    // pooling later on
    public void Spawn()
    {
        var npc = NPCPool.Instance.GetNPC();
        if (npc == null) 
            return;
        npc.transform.position = transform.position;
    }
}

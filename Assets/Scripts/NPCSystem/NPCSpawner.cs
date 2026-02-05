using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    private float spawnInterval = 5f;
    private float timeSinceLastSpawn = 0f;
    private bool allnpcretunred = false;
    private bool firstStartup = true;

    private void Update()
    {
        if(GameState.inTutorial && TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.currentStep == TutorialStep.WaitingForNPCSpawn)
            {
                SpawnTutorialNPC();
                TutorialManager.Instance.OnNPCSpawned();
            }
            return;
        }

        if (allnpcretunred)
            return;

        if (GameState.dayEnded)
        {
            if (NPCPool.Instance.AllNPCsReturned())
            {
                TimeManager.Instance.ShowDaySummary();
                allnpcretunred = true;
            }
            return;
        }

        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && ChairManager.Instance.GetFreeChair() != null && NPCPool.Instance.HasNPCs)
        {
            Spawn();
            timeSinceLastSpawn = 0f;
        }
    }
    public void Spawn()
    {
        if (firstStartup)
        {
            ChairManager.Instance.Initiate();
            firstStartup = false;
        }
            
        var npc = NPCPool.Instance.GetNPC();
        if (npc == null) 
            return;
        npc.transform.position = transform.position;
        var controller = npc.GetComponent<NPCController>();
        controller.Initialize();
        TimeManager.Instance.TrackCostumers();
    }

    public void SpawnTutorialNPC()
    {
        if (firstStartup)
        {
            ChairManager.Instance.Initiate();
            firstStartup = false;
        }
        
        var npc = NPCPool.Instance.GetTutorialNPC();
        if (npc == null) 
            return;
        npc.transform.position = transform.position;
        var controller = npc.GetComponent<NPCController>();
        controller.Initialize();
    }

}

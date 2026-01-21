using System.Collections.Generic;
using UnityEngine;

public class NPCUIManager : MonoBehaviour
{
    public static NPCUIManager Instance;
    [SerializeField] private NPCOverheadUI prefab;
    [SerializeField] private int initialPool = 10;
    
    private Queue<NPCOverheadUI> pool = new Queue<NPCOverheadUI>();

    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        
        Instance = this;
        
        for (int i = 0; i < initialPool; i++)
        {
            var ui = Instantiate(prefab, transform);
            ui.gameObject.SetActive(false);
            pool.Enqueue(ui);
        }
    }
    
    public NPCOverheadUI GetNPCUI()
    {
        if (pool.Count > 0)
        {
            var ui = pool.Dequeue();
            ui.gameObject.SetActive(true);
            return ui;
        }
        
        var newUI = Instantiate(prefab, transform);
        return newUI;
    }

    public void ReturnUI(NPCOverheadUI ui)
    {
        ui.gameObject.SetActive(false);
        pool.Enqueue(ui);
    }
}

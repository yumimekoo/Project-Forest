using System;
using UnityEngine;
using UnityEngine.UI;

public class NPCOverheadUI : MonoBehaviour
{
    [Header("References")]
    public Image radialFill;
    public Image centerIcon;
    public Canvas canvas;
    
    [Header("Icons")]
    [SerializeField] private Sprite takeOrderIcon;
    [SerializeField] private Sprite drinkingIcon;
    [SerializeField] private Sprite talkBubbleIcon;
    [SerializeField] private Sprite surpriseOrderIcon;

    [Header("Settings")] 
    [SerializeField] private int offset;

    private NPCController npc;
    private float maxStateTime;

    public void Init(NPCController controller)
    {
        npc = controller;
        transform.SetParent(npc.transform);
        transform.localPosition = Vector3.up * offset;
        transform.localRotation = Quaternion.identity;
        canvas.transform.localScale = Vector3.one * 0.02f;
    }

    private void LateUpdate()
    {
        if (!npc) return;
        UpdateVisibility();
        UpdateProgress();
        UpdateIcon();
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.x = 0; // Y ignorieren, nur nach vorne drehen

        if (direction.sqrMagnitude > 0.001f) // Schutz vor Null-Vektor
            transform.rotation = Quaternion.LookRotation(direction);
    }
    
    public void ResetUI()
    {
        radialFill.fillAmount = 1f;
        centerIcon.sprite = null;
        transform.SetParent(null);
    }

    private void UpdateVisibility()
    {
        canvas.enabled = npc.State != NPCState.Leaving && npc.State != NPCState.Walking;
    }

    private void UpdateProgress()
    {
        if (maxStateTime <= 0f) return;
        
        float progress = npc.StateTimer / maxStateTime;
        radialFill.fillAmount = progress;
        radialFill.color = Color.Lerp(Color.red, Color.green, progress);
    }

    private void UpdateIcon()
    {
        switch (npc.State)
        {
            case NPCState.Sitting:
                centerIcon.sprite = takeOrderIcon;
                break;
            case NPCState.WaitingForDrink:
                centerIcon.sprite = npc.currentOrder?.requestedDrink ? npc.currentOrder.requestedDrink.icon : surpriseOrderIcon;
                break;
            case NPCState.Drinking:
                centerIcon.sprite = npc.HasTalked ? drinkingIcon : talkBubbleIcon;
                break;
            default:
                centerIcon.sprite = null;
                break;
        }
    }
    
    public void OnStateChanged(float duration) => maxStateTime = duration;

}

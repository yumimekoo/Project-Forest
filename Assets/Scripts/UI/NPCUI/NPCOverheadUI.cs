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

    [SerializeField] private Color colorA;
    [SerializeField] private Color colorB;

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
        canvas.transform.localScale = Vector3.one * 0.015f;
    }

    private void LateUpdate()
    {
        if (!npc) return;
        UpdateVisibility();
        UpdateProgress();
        UpdateIcon();
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.x = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(0f, 180f, 0f);
        }
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

        Color start = colorA; start.a = 1f;
        Color end   = colorB; end.a = 1f;

        float t = Mathf.InverseLerp(0.1f, 0.4f, progress);

        Color resultColor = Color.Lerp(end.linear, start.linear, t).gamma;
        
        radialFill.fillAmount = progress;
        radialFill.color = resultColor;
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

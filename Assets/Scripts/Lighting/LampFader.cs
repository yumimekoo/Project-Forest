using System.Collections.Generic;
using UnityEngine;

public class LampFader : MonoBehaviour
{
    [Header("Light")] 
    public Light lampLight;
    public float dayIntensity = 0f;
    public float nightIntensity = 1.8f;

    [Header("Emission")]
    public Renderer[] glowRenderers;
    public string emissionProperty = "_EmissionColor";
    public Color dayEmissionColor = Color.black;
    public Color nightEmissionColor = new Color(1f, 0.75f, 0.35f);
    public float emissionIntensity = 1f;

    private readonly Dictionary<Renderer, MaterialPropertyBlock> mpbs = new();

    private void Awake()
    {
        if(glowRenderers == null || glowRenderers.Length == 0)
            glowRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        foreach (var r in glowRenderers)
        {
            if (!r) continue;
            mpbs[r] = new MaterialPropertyBlock();
        }
    }

    public void Apply(float t)
    {
        if(lampLight)
            lampLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, t);
        
        Color emission = Color.Lerp(dayEmissionColor, nightEmissionColor, t);

        foreach (var kvp in mpbs)
        {
            var r = kvp.Key;
            if(!r) continue;
            
            var mpb = kvp.Value;
            r.GetPropertyBlock(mpb);
            mpb.SetColor(emissionProperty, emission * emissionIntensity);
            r.SetPropertyBlock(mpb);
        }
    }
}

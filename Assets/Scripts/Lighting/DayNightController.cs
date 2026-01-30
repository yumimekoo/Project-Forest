using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class DayNightController : MonoBehaviour
{
    [Header("Profiles")]
    public DayNightProfileSO dayProfile;
    public DayNightProfileSO nightProfile;
    
    [Header("Scene References")]
    public Light directionalLight;
    public bool setSkyBoyMaterial = true;
    
    [Header("Transition")]
    [Min(0.1f)] public float transitionDuration = 6f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Lamps")] 
    public LayerMask lampsLayer;
    public bool autoFindLampsOnStart = true;

    private readonly List<LampFader> lamps = new();
    private Coroutine running;
    
    [Header("Universal Render Pipeline")]
    public Volume urpVolume;
    private ColorAdjustments colorAdjustments;

    private void OnEnable()
    {
        if (!TimeManager.Instance) return;
        Debug.Log("TimeManager ref enabled");
        TimeManager.Instance.OnNightTriggered += TransitionToNight;
        TimeManager.Instance.OnNewDayStarted += TransitionToDay;
    }
    
    private void OnDisable()
    {
        if (!TimeManager.Instance) return;
        Debug.Log("TimeManager ref disabled");
        TimeManager.Instance.OnNightTriggered -= TransitionToNight;
        TimeManager.Instance.OnNewDayStarted -= TransitionToDay;
    }

    private void Awake()
    {
        if(urpVolume && urpVolume.profile)
            urpVolume.profile.TryGet(out colorAdjustments);
    }

    private void Start()
    {
        if (autoFindLampsOnStart)
            RebuildLampList();
        
        ApplyProfile(dayProfile, immediate: true);
        ApplyLamps(0f);
    }

    public void RebuildLampList()
    {
        lamps.Clear();

        foreach (var lampFader in FindObjectsByType<LampFader>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!lampFader) continue;
            int layerBit = 1 << lampFader.gameObject.layer;
            if ((lampsLayer.value & layerBit) != 0)
                lamps.Add(lampFader);
        }
    }

    public void TransitionToNight()
    {
        StartTransition(dayProfile, nightProfile, lampLightEnd: 1f);
    }

    public void TransitionToDay()
    {
        ApplyProfile(dayProfile, immediate: true);
    }

    private void StartTransition(DayNightProfileSO from, DayNightProfileSO to, float lampLightEnd)
    {
        if(running != null) StopCoroutine(running);
        running = StartCoroutine(Transition(from, to, lampLightEnd));
    }

    private IEnumerator Transition(DayNightProfileSO from, DayNightProfileSO to, float lampLightEnd)
    {
        if (setSkyBoyMaterial && to.skyboxMaterial)
            RenderSettings.skybox = to.skyboxMaterial;

        float t = 0f;
        float startLamps = (lampLightEnd > 0.5f) ? 0f : 1f;
        float endLamps = lampLightEnd;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, transitionDuration);
            float k = ease.Evaluate(Mathf.Clamp01(t));
            
            ApplyLerped(from, to, k);
            ApplyLamps(Mathf.Lerp(startLamps, endLamps, k));

            yield return null;
        }
        
        ApplyProfile(to, immediate: true);
        ApplyLamps(lampLightEnd);
        running = null;
    }

    private void ApplyLerped(DayNightProfileSO a, DayNightProfileSO b, float t)
    {
        if (directionalLight)
        {
            directionalLight.color = Color.Lerp(a.directionalColor, b.directionalColor, t);
            directionalLight.intensity = Mathf.Lerp(a.directionalIntensity, b.directionalIntensity, t);

            var rotA = Quaternion.Euler(a.directionalEuler);
            var rotB = Quaternion.Euler(b.directionalEuler);
            directionalLight.transform.rotation = Quaternion.Slerp(rotA, rotB, t);
        }

        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = Color.Lerp(a.ambientSkyColor, b.ambientSkyColor, t);
        RenderSettings.ambientEquatorColor = Color.Lerp(a.ambientEquatorColor, b.ambientEquatorColor, t);
        RenderSettings.ambientGroundColor = Color.Lerp(a.ambientGroundColor, b.ambientGroundColor, t);
        RenderSettings.ambientIntensity = Mathf.Lerp(a.ambientIntensity, b.ambientIntensity, t);
        
        RenderSettings.fog = (t < 0.5f) ? a.useFog : b.useFog;
        RenderSettings.fogColor = Color.Lerp(a.fogColor, b.fogColor, t);
        RenderSettings.fogDensity = Mathf.Lerp(a.fogDensity, b.fogDensity, t);
        
        if(RenderSettings.skybox)
        {
            if (RenderSettings.skybox.HasProperty("_Exposure"))
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(a.skyboxExposure, b.skyboxExposure, t));
        }
        
        if (colorAdjustments)
            colorAdjustments.postExposure.value = Mathf.Lerp(a.postExposure, b.postExposure, t);
    }

    private void ApplyProfile(DayNightProfileSO p, bool immediate = false)
    {
        if (!p) return;
        
        if(setSkyBoyMaterial && p.skyboxMaterial)
            RenderSettings.skybox = p.skyboxMaterial;

        if (directionalLight)
        {
            directionalLight.color = p.directionalColor;
            directionalLight.intensity = p.directionalIntensity;
            directionalLight.transform.rotation = Quaternion.Euler(p.directionalEuler);
        }
        
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = p.ambientSkyColor;
        RenderSettings.ambientEquatorColor = p.ambientEquatorColor;
        RenderSettings.ambientGroundColor = p.ambientGroundColor;
        RenderSettings.ambientIntensity = p.ambientIntensity;
        
        RenderSettings.fog = p.useFog;
        RenderSettings.fogColor = p.fogColor;
        RenderSettings.fogDensity = p.fogDensity;
        
        if(RenderSettings.skybox && RenderSettings.skybox.HasProperty("_Exposure"))
                RenderSettings.skybox.SetFloat("_Exposure", p.skyboxExposure);
        
        if (colorAdjustments)
            colorAdjustments.postExposure.value = p.postExposure;
    }

    private void ApplyLamps(float t)
    {
        for (int i = 0; i < lamps.Count; i++)
        {
            if(lamps[i])
                lamps[i].Apply(t);
        }
    }
}

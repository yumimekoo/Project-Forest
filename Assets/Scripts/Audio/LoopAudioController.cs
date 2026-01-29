using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LoopAudioController : MonoBehaviour
{
    [Header("Loop Audio")]
    [SerializeField] private SoundSO loopSound;
    [SerializeField] private float fadeInSeconds = 0.25f;
    [SerializeField] private float fadeOutSeconds = 0.25f;
    
    [SerializeField] private AudioSource source;
    
    private Coroutine fadeRoutine;
    private bool isLoopRequested;

    private void Awake()
    {
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 1f;
    }

    private void Start()
    {
        ApplyLoopSoundSettings();
        source.volume = 0f;
        source.Stop();
    }

    private void ApplyLoopSoundSettings()
    {
        if (!loopSound) return;
        
        var clip = loopSound.GetRandomClip();
        source.clip = clip;
        
        if(loopSound.mixerGroup) source.outputAudioMixerGroup = loopSound.mixerGroup;
        
        source.spatialBlend = loopSound.spacial3D ? 1f : 0f;
        source.maxDistance = loopSound.maxDistance;
        source.minDistance = loopSound.minDistance;
        source.pitch = loopSound.GetRandomPitch();
        source.loop = loopSound.loop;
    }

    public void StartLoop()
    {
        isLoopRequested = true;
        if(!loopSound || !source.clip) ApplyLoopSoundSettings();
        if(fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(loopSound ? loopSound.volume : 1f, fadeInSeconds, ensurePlaying: true));
    }

    public void StopLoop()
    {
        isLoopRequested = false;
        
        if(fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(0f, fadeOutSeconds, ensurePlaying: false));
    }

    public bool IsRunning() => isLoopRequested;

    private IEnumerator FadeTo(float targetVolume, float seconds, bool ensurePlaying)
    {
        if (ensurePlaying && !source.isPlaying)
        {
            if(!source.clip) ApplyLoopSoundSettings();
            if(source.clip) source.Play();
        }

        float startVolume = source.volume;
        float t = 0f;

        while (t < seconds)
        {
            if(!ensurePlaying && isLoopRequested) yield break;
            
            t += Time.unscaledDeltaTime;
            float a = seconds <= 0.0001f ? 1f : Mathf.Clamp01(t / seconds);
            source.volume = Mathf.Lerp(startVolume, targetVolume, a);
            yield return null;
        }
        
        source.volume = targetVolume;

        if (!ensurePlaying && Mathf.Approximately(targetVolume, 0f) && !isLoopRequested)
        {
            source.Stop();
        } 
        fadeRoutine = null;
    }
}

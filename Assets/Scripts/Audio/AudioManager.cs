using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    [Header("Mixer Routing")] 
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    
    [Header("Music")]
    [SerializeField] private float defaultMusicFadeTime = 2f;
    
    [Header("SFX Pool")]
    [SerializeField] private int defaultSFXPoolSize = 10;
    
    [Header("Pause Feel (Music)")]
    [SerializeField] private float pauseMusicVolumeMultiplier = 0.75f; // z.B. 0.7 - 0.85
    [SerializeField] private float pauseMusicPitchMultiplier = 0.6f;  // z.B. 0.88 - 0.95
    [SerializeField] private float pauseTransitionSeconds = 1f;

    private Coroutine pauseRoutine;
    private float pauseBlend = 0f;

    private AudioSource musicA;
    private AudioSource musicB;
    private bool aIsActive = true;
    private Coroutine musicFadeRoutine;
    private float musicABaseVol = 1f;
    private float musicBBaseVol = 0f;
    
    private AudioSource[] sfxPool;
    private int sfxPoolIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupMusicSources();
        SetupSfxPool();
    }

    private void SetupMusicSources()
    {
        musicA = gameObject.AddComponent<AudioSource>();
        musicB = gameObject.AddComponent<AudioSource>();

        ConfigureMusicSource(musicA);
        ConfigureMusicSource(musicB);
        
        musicA.volume = 1f;
        musicB.volume = 0f;
        
        musicABaseVol = 1f;
        musicBBaseVol = 0f;
        ApplyPauseToMusicSources();
    }

    private void ConfigureMusicSource(AudioSource src)
    {
        src.playOnAwake = false;
        src.loop = true;
        src.spatialBlend = 0f;
        src.outputAudioMixerGroup = musicGroup;
    }

    private void SetupSfxPool()
    {
        sfxPool = new AudioSource[Mathf.Max(1, defaultSFXPoolSize)];

        for (int i = 0; i < sfxPool.Length; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;
            src.outputAudioMixerGroup = sfxGroup;
            sfxPool[i] = src;
        }
    }

    public void Play(SoundSO sound)
    {
        if (!sound) return;
        PlayInternal(sound, null, true);
    }

    public void PlayAt(SoundSO sound, Transform target)
    {
        if (!sound || !target) return;
        PlayInternal(sound, target, false);
    }
    
    public void PlayAt(SoundSO sound, Vector3 position)
    {
        if (!sound) return;
        var src = GetNextSfxSource();
        ApplySoundToSource(sound, src, for3D: true);
        
        src.transform.position = position;
        src.PlayOneShot(sound.GetRandomClip(), sound.volume);
    }

    private void PlayInternal(SoundSO sound, Transform target, bool force2D)
    {
        var clip = sound.GetRandomClip();
        if (!clip) return;

        var src = GetNextSfxSource();

        bool use3D = !force2D && sound.spacial3D;
        ApplySoundToSource(sound, src, use3D);
        
        if(!use3D && uiGroup && sound.mixerGroup == uiGroup)
            src.outputAudioMixerGroup = uiGroup;
        else 
            src.outputAudioMixerGroup = sound.mixerGroup ? sound.mixerGroup : sfxGroup;
        
        if(target)
            src.transform.position = target.position;
        
        src.PlayOneShot(clip, sound.volume);
    }

    private AudioSource GetNextSfxSource()
    {
        var src = sfxPool[sfxPoolIndex];
        sfxPoolIndex = (sfxPoolIndex + 1) % sfxPool.Length;
        return src;
    }

    private void ApplySoundToSource(SoundSO sound, AudioSource src, bool for3D)
    {
        src.pitch = sound.GetRandomPitch();
        src.loop = false;

        if (for3D)
        {
            src.spatialBlend = 1f;
            src.maxDistance = sound.maxDistance;
            src.minDistance = sound.minDistance;
        }
        else
        {
            src.spatialBlend = 0f;
        }
        
        src.outputAudioMixerGroup = sound.mixerGroup ? sound.mixerGroup : sfxGroup;
    }

    public void CrossfadeMusic(AudioClip nextClip, float fadeSeconds = -1f, bool syncWithCurrent = false)
    {
        if (!nextClip) return;
        if(fadeSeconds < 0f) fadeSeconds = defaultMusicFadeTime;
        
        var from = aIsActive ? musicA : musicB;
        var to = aIsActive ? musicB : musicA;

        if (to.clip == nextClip && to.isPlaying) return;

        to.clip = nextClip;
        to.loop = true;

        if (syncWithCurrent && from.clip && from.isPlaying)
        {
            float t = from.time;
            if(nextClip.length > 0.01f) to.time = Mathf.Repeat(t, nextClip.length);
        } else to.time = 0f;
        
        to.volume = 0f;
        if(!to.isPlaying) to.Play();
        if(musicFadeRoutine != null) StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = StartCoroutine(FadeRoutine(from, to, fadeSeconds));
        
        aIsActive = !aIsActive;
    }

    public void SetMusicBlend(float t)
    {
        t = Mathf.Clamp01(t);
        musicABaseVol = 1f - t;
        musicBBaseVol = t;
        ApplyPauseToMusicSources();
        
        if(musicA.clip && !musicA.isPlaying) musicA.Play();
        if(musicB.clip && !musicB.isPlaying) musicB.Play();
    }

    public void StopMusic(float fadeSeconds = -1)
    {
        if(fadeSeconds <= 0f) fadeSeconds = defaultMusicFadeTime;
        
        if(musicFadeRoutine != null) StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = StartCoroutine(StopMusicRoutine(fadeSeconds));
    }

    private IEnumerator FadeRoutine(AudioSource from, AudioSource to, float seconds)
    {
        float startFrom = from == musicA ? musicABaseVol : musicBBaseVol;
        float startTo   = to   == musicA ? musicABaseVol : musicBBaseVol;

        float time = 0f;
        while (time < seconds)
        {
            time += Time.unscaledDeltaTime;
            float a = seconds <= 0.0001f ? 1f : Mathf.Clamp01(time / seconds);
            
            float fromBase = Mathf.Lerp(startFrom, 0f, a);
            float toBase = Mathf.Lerp(startTo, 1f, a);
            
            if (from == musicA) musicABaseVol = fromBase; else musicBBaseVol = fromBase;
            if (to   == musicA) musicABaseVol = toBase;   else musicBBaseVol = toBase;
            
            ApplyPauseToMusicSources();
            yield return null;
        }

        if (from == musicA) musicABaseVol = 0f; else musicBBaseVol = 0f;
        if (to   == musicA) musicABaseVol = 1f; else musicBBaseVol = 1f;
        ApplyPauseToMusicSources();
        
        from.Stop();
    }

    private IEnumerator StopMusicRoutine(float seconds)
    {
        float aStart = musicABaseVol;
        float bStart = musicBBaseVol;

        float time = 0f;
        while (time < seconds)
        {
            time += Time.unscaledDeltaTime;
            float a = seconds <= 0.0001f ? 1f : Mathf.Clamp01(time / seconds);
            
            musicABaseVol = Mathf.Lerp(aStart, 0f, a);
            musicBBaseVol = Mathf.Lerp(bStart, 0f, a);
            ApplyPauseToMusicSources();
            
            yield return null;
        }
        
        musicA.Stop();
        musicB.Stop();
        musicABaseVol = 0f;
        musicBBaseVol = 0f;
        ApplyPauseToMusicSources();
    }
    
    public void SetPausedAudio(bool paused, float seconds = -1f)
    {
        if (seconds < 0f) seconds = pauseTransitionSeconds;

        if (pauseRoutine != null) StopCoroutine(pauseRoutine);
        pauseRoutine = StartCoroutine(PauseMusicRoutine(paused ? 1f : 0f, seconds));
    }
    
    private IEnumerator PauseMusicRoutine(float targetBlend, float seconds)
    {
        float start = pauseBlend;
        float time = 0f;

        while (time < seconds)
        {
            time += Time.unscaledDeltaTime;
            float a = seconds <= 0.0001f ? 1f : Mathf.Clamp01(time / seconds);
            pauseBlend = Mathf.Lerp(start, targetBlend, a);
            ApplyPauseToMusicSources();
            yield return null;
        }

        pauseBlend = targetBlend;
        ApplyPauseToMusicSources();
    }
    
    private void ApplyPauseToMusicSources()
    {
        float volMul = Mathf.Lerp(1f, pauseMusicVolumeMultiplier, pauseBlend);
        float pitchMul = Mathf.Lerp(1f, pauseMusicPitchMultiplier, pauseBlend);
        
        musicA.volume = musicABaseVol * volMul;
        musicB.volume = musicBBaseVol * volMul;

        musicA.pitch = pitchMul;
        musicB.pitch = pitchMul;
    }

}

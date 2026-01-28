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

    private AudioSource musicA;
    private AudioSource musicB;
    private bool aIsActive = true;
    private Coroutine musicFadeRoutine;
    
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
        musicA.volume = 1f - t;
        musicB.volume = t;
        
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
        float startFrom = from.volume;
        float startTo = to.volume;

        float time = 0f;
        while (time < seconds)
        {
            time += Time.unscaledDeltaTime;
            float a = seconds <= 0.0001f ? 1f : Mathf.Clamp01(time / seconds);
            
            from.volume = Mathf.Lerp(startFrom, 0f, a);
            to.volume = Mathf.Lerp(startTo, 1f, a);
            
            yield return null;
        }

        from.volume = 0f;
        to.volume = 1f;
        
        from.Stop();
    }

    private IEnumerator StopMusicRoutine(float seconds)
    {
        float aStart = musicA.volume;
        float bStart = musicB.volume;

        float time = 0f;
        while (time < seconds)
        {
            time += Time.unscaledDeltaTime;
            float a = seconds <= 0.0001f ? 1f : Mathf.Clamp01(time / seconds);
            
            musicA.volume = Mathf.Lerp(aStart, 0f, a);
            musicB.volume = Mathf.Lerp(bStart, 0f, a);
            
            yield return null;
        }
        
        musicA.Stop();
        musicB.Stop();
        musicA.volume = 0f;
        musicB.volume = 0f;
    }

}

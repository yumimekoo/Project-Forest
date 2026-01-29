using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundSO", menuName = "Scriptable Objects/SoundSO")]
public class SoundSO : ScriptableObject
{
    [Header("Audio Clip")] 
    public AudioClip[] clips;
    
    [Header("Settings")] 
    [Range(0f, 1f)] public float volume = 1f;
    public Vector2 pitchRange = new Vector2(1f, 1f);
    
    [Header("Playback")]
    public bool loop = false;
    public bool spacial3D = false;

    [Header("3D")] 
    public float minDistance = 1f;
    public float maxDistance = 10f;
    
    [Header("Routing")]
    public AudioMixerGroup mixerGroup;

    public AudioClip GetRandomClip()
    {
        if(clips.Length == 0 || clips == null) return null;
        if(clips.Length == 1) return clips[0];
        return clips[Random.Range(0, clips.Length)];
    }
    
    public float GetRandomPitch() => Random.Range(pitchRange.x, pitchRange.y);
}    
    
    
    
    

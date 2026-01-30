using UnityEngine;

[CreateAssetMenu(fileName = "DayNightProfileSO", menuName = "Scriptable Objects/DayNightProfileSO")]
public class DayNightProfileSO : ScriptableObject
{
    [Header("Directional Light (Sun/Moon")]
    public Color directionalColor = Color.white;
    [Min(0f)] public float directionalIntensity = 1f;
    public Vector3 directionalEuler = new Vector3(50f, -30f, 0f);
    
    [Header("Ambient Light")]
    public Color ambientSkyColor = Color.gray;
    public Color ambientEquatorColor = Color.gray;
    public Color ambientGroundColor = Color.gray;
    [Min(0f)] public float ambientIntensity = 1f;
    
    [Header("Fog")]
    public bool useFog = true;
    public Color fogColor = new Color(0.6f, 0.7f, 0.7f, 1f);
    [Min(0f)] public float fogDensity = 0.01f;
    
    [Header("Skybox")]
    public Material skyboxMaterial;
    public float skyboxExposure = 1f;
    
    [Header("Post Processing")]
    public float postExposure = 0f;

}

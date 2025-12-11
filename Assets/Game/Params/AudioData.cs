using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : ScriptableObject
{
    [SerializeField] private AudioClip clip;
    [SerializeField] [Range(0f, 1f)]　private float volume = 1f;
    [SerializeField] [Range(0.1f, 3f)]　private float pitch = 1f;
    
    public AudioClip Clip => clip;
    public float Volume => volume;
    public float Pitch => pitch;
}

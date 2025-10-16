using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum SoundType
    {
        music,
        sfx
    }
    public SoundType type;
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    public bool loop;
    [HideInInspector]
    public AudioSource source;
}

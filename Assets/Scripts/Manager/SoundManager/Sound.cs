using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip audioClip;
    public string nameAudio;
    public bool playOnAwake;
    public bool loop;

    [Range(0f, 1f)]
    public float volume;

    public AudioSource source;

}

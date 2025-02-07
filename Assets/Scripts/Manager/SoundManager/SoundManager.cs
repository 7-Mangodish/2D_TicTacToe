using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] listSound;
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }
    
    public enum SoundType {
        backround,
        sfx,
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);

        foreach(Sound x in listSound) {
            x.source = this.AddComponent<AudioSource>();

            x.source.clip = x.audioClip;
            x.source.name = x.nameAudio;
            x.source.volume = x.volume;
            x.source.playOnAwake = x.playOnAwake;
            x.source.loop = x.loop;

        }
    }
    void Start()
    {
        //if (SceneManager.GetActiveScene().buildIndex == 0)
        Play("Background");
    }

    void Update()
    {
        
    }

    public void Play(string nameAudio) {
        Sound res = null;
        foreach(Sound sound in listSound) {
            if(sound.nameAudio == nameAudio) {
                res = sound; break;
            }
        }
        if (res != null)
            res.source.Play();
        else
            Debug.Log("Cant find audio: " + res.nameAudio);
    }

    public void ChangeVolume(SoundType type, float volume) {
        if(type == SoundType.backround) {
            foreach (Sound sound in listSound) {
                if (sound.nameAudio == "Background") {
                    sound.source.volume = volume;
                }
            }
        }
        else {
            foreach (Sound sound in listSound) {
                if (sound.nameAudio != "Background") {
                    sound.source.volume = volume;
                }
            }
        }

    }
}

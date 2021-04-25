using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //the instance is used to check if there is another audio manager
    public static AudioManager instance;
    public Sound[] sounds;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound snd in sounds)
        {
            snd.source = gameObject.AddComponent<AudioSource>();
            snd.source.clip = snd.sound;
            snd.source.volume = snd.volume;
            snd.source.pitch = snd.pitch;
            snd.source.loop = snd.loopable;
        }
    }

    public void PlaySound(string name)
    {
        Sound snd = Array.Find(sounds, sound => sound.name == name);
        if (snd == null)
        {
            Debug.LogError("PlaySound: no sound found for " + snd.name);
            return;
        }

        if (snd.source.isPlaying && !snd.canOverlap)
        {
            Debug.Log("Sound is already playing");
            return;
        }
        snd.source.Play();
    }

    public void StopSound(string name)
    {
        Sound snd = Array.Find(sounds, sound => sound.name == name);
        if (snd == null)
        {
            Debug.LogError("StopSound: no sound found for " + snd.name);
            return;
        }
        snd.source.Stop();
        Debug.Log("sound stopped");
    }

    public void StopAllSounds()
    {
        foreach (Sound snd in sounds)
        {
            //StopSound(snd);
            snd.source.Stop();
        }
    }
    
    public void Start()
    {
        //FindObjectOfType<AudioManager>().PlaySound("MenuTheme");
    }
}

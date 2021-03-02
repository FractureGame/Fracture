using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public Sound[] sounds;
    // Start is called before the first frame update
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

    // Update is called once per frame
    public void PlaySound(string name)
    {
        Sound snd = Array.Find(sounds, sound => sound.name == name);
        if (snd == null)
        {
            return;
        }
        snd.source.Play();
    }

    public void StopSound(string name)
    {
        Sound snd = Array.Find(sounds, sound => sound.name == name);
        if (snd == null)
        {
            return;
        }
        snd.source.Stop();
    }
    
    public void Start()
    {
        FindObjectOfType<AudioManager>().PlaySound("MenuTheme");
    }
}

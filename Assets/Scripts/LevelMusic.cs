using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusic : MonoBehaviour
{
    private AudioManager audioManager;

    private int level;
    // Start is called before the first frame update
    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        
        audioManager.StopAllSounds();
        level = SceneManager.GetActiveScene().buildIndex - 2;
        if (level >= 1 && level <= 3)
        {
            audioManager.PlaySound("LevelMusic1");
        }
        else if (level == 6)
        {
            audioManager.PlaySound("BossIntro");
        }
        else
        {
            audioManager.PlaySound("LevelMusic2");

        }
    }

    void Update()
    {
        if (level == 6)
        {
            if (!audioManager.sounds[5].source.isPlaying)
            {
                audioManager.PlaySound("BossLoop");
                enabled = false;
            }
        }

        else
        {
            enabled = false;
        }
    }
}

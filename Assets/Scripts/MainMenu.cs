using System;
using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager am;
    public void Start()
    {
        am = FindObjectOfType<AudioManager>();
        if (!am.sounds[0].source.isPlaying)
        {
            am.StopAllSounds();
            FindObjectOfType<AudioManager>().PlaySound("MenuTheme");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

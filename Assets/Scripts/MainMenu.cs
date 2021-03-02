using System;
using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        FindObjectOfType<AudioManager>().PlaySound("MenuTheme");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

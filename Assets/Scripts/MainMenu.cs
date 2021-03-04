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
        //Screen.SetResolution(1280,720,FullScreenMode.FullScreenWindow);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

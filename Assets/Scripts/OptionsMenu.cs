using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public Resolution[] resolutions;

    private int i;
    private bool fullscreen;
    public void SetRes(int val)
    {
        i = val;
    }
    public void SetScreenMode(bool b)
    {
        fullscreen = b;
    }
    public void ApplyChangeRes()
    {
        if (fullscreen)
        {
            Screen.SetResolution(resolutions[i].width,resolutions[i].height,FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(resolutions[i].width,resolutions[i].height,FullScreenMode.Windowed);
        }
    }

    public void Start()
    {
        //i = 1;
        //fullscreen = false;
    }
}

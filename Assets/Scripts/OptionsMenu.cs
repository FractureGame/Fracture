﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Resolution = UnityEngine.Resolution;

public class OptionsMenu : MonoBehaviour
{
    public Toggle fullscreenToggle;
    UnityEngine.Resolution[] resolutions;
    public GameObject resOptions;
    private UnityEngine.Resolution fhd = new UnityEngine.Resolution();
    private UnityEngine.Resolution hd = new UnityEngine.Resolution();
     

    public void SetRes(int resIndex)
    {
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height,Screen.fullScreen);
    }
    public void SetScreenMode(bool b)
    {
        Screen.fullScreen = b;
    }

    public void Awake()
    {
        //Fullscreen stuff
        if (Screen.fullScreen)
        {
            Debug.Log("game is in fullscreen");
        }
        fullscreenToggle.isOn = Screen.fullScreen;
        //Resolution stuff
        TMP_Dropdown resolutionsDropdown = resOptions.GetComponent<TMP_Dropdown>();
        //resolutions = Screen.resolutions;
        resolutions = new UnityEngine.Resolution[2];
        fhd.width = 1920;
        fhd.height = 1080;
        hd.width = 1280;
        hd.height = 720;
        resolutions[0] = hd;
        resolutions[1] = fhd;
        resolutionsDropdown.ClearOptions();
        List<string> resStrings = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resStrings.Add(option);
            if (resolutions[i].height == Screen.height &&
                resolutions[i].width == Screen.width)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(resStrings);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }
}

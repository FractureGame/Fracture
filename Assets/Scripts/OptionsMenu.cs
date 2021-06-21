using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OptionsMenu : MonoBehaviour
{
    public Toggle fullscreenToggle;
    UnityEngine.Resolution[] resolutions;
    public GameObject resOptions;
    public void SetRes(int val)
    {
        
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
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> resStrings = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resStrings.Add(option);
            if (resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].width == Screen.currentResolution.width)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionsDropdown.AddOptions(resStrings);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }
}

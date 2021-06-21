using System;
using System.Collections;
using System.Collections.Generic;
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
        if (Screen.fullScreen)
        {
            Debug.Log("game is in fullscreen");
        }
        fullscreenToggle.isOn = Screen.fullScreen;
        TMP_Dropdown resolutionsDropdown = resOptions.GetComponent<TMP_Dropdown>();
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> resStrings = new List<string>();
        foreach (var e in resolutions)
        {
            string option = e.width + " x " + e.height;
            Debug.Log(option);
            resStrings.Add(option);
        }
        resolutionsDropdown.AddOptions(resStrings);
    }
}

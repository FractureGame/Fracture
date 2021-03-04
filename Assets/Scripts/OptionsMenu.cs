using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public bool fullscreen;
    public Tuple<int, int> resolution;
    public static Tuple<int,int>[] resolutions = new Tuple<int,int>[2] {Tuple.Create(1920,1080),Tuple.Create(1440,1080)};
    public void ChangeFullScreen(bool val)
    {
        fullscreen = val;
    }
    public void ChooseRes(int i)
    {
        resolution = resolutions[i];
    }
    public void ChangeRes()
    {
        Screen.SetResolution(resolution.Item1, resolution.Item2, fullscreen);
    }

    public void Start()
    {
        fullscreen = true;
        resolution = resolutions[0];
    }
}

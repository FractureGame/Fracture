using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void ChangeRes(int val)
    {
        switch (val)
        {
            case 0:
                Screen.SetResolution(1920,1080,true);
                return;
            case 1:
                Screen.SetResolution(1440,1080,true);
                return;
        }
    }
}

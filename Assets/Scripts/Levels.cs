using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Levels
{
    // Start is called before the first frame update
    public static readonly Dictionary<string,string> scenes = new Dictionary<string,string>()
    {
        {"HLevel1","Assets/Scenes/HLevel1"},
        {"Vlevel2","Assets/Scenes/VLevel2"}, 
        {"VLevel3","Assets/Scenes/VLevel3"}
    };
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Levels
{
    // Start is called before the first frame update
    public static readonly Dictionary<string,int> scenes = new Dictionary<string,int>()
    {
        {"HLevel1",3},
        {"HLevel2",4},
        {"VLevel2",5}, 
        {"VLevel3",6}
    };
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Levels
{
    // Start is called before the first frame update
    public static readonly Dictionary<string,int> scenes = new Dictionary<string,int>()
    {
        {"Level 1",3},
        {"Level 2",4},
        {"Level 3",5}
    };
}

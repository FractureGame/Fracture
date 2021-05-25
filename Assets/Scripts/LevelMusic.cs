using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<AudioManager>().StopSound("MenuTheme");
        FindObjectOfType<AudioManager>().PlaySound("LevelMusic1");
    }
    
}

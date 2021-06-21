using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public Dictionary<string, KeyCode> actionKeys;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        actionKeys = new Dictionary<string, KeyCode>
        {
            ["Jump"] = KeyCode.Space,
            ["Switch"] = KeyCode.S,
            ["Climb"] = KeyCode.C,
            ["Attack"] = KeyCode.A,
            ["Dash"] = KeyCode.LeftShift
        };
    }

    private void Update()
    {
        
    }

    

    
    public bool GetKeyDown(string action)
    {
        if (!actionKeys.ContainsKey(action))
        {
            Debug.LogError("GetKeyDown: " + action + " is not an existing action");
            return false;
        }

        return Input.GetKeyDown(actionKeys[action]);
    }
}

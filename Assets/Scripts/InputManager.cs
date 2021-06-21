using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static Dictionary<string, KeyCode> actionKeys;
    public GameObject keybindingPrefab;
    private void Start()
    {
        actionKeys = new Dictionary<string, KeyCode>
        {
            ["Jump"] = KeyCode.Space,
            ["Switch"] = KeyCode.S,
            ["Climb"] = KeyCode.C,
            ["Attack"] = KeyCode.A,
            ["Dash"] = KeyCode.LeftShift
        };
        GameObject parent = GameObject.Find("Keybinding Menu");
        foreach (var kvp in actionKeys)
        {
            GameObject binding = Instantiate(keybindingPrefab, parent.transform);
            binding.transform.Find("Action Text").GetComponent<Text>().text = kvp.Key;
            binding.transform.Find("Button/Key Text").GetComponent<Text>().text = kvp.Value.ToString();
        }
    }

    public static bool GetKeyDown(string action)
    {
        if (!actionKeys.ContainsKey(action))
        {
            Debug.LogError("GetKeyDown: " + action + " is not an existing action");
            return false;
        }

        return Input.GetKeyDown(actionKeys[action]);
    }
}

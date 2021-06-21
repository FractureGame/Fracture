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
    private string actionToRebind;
    Array kcs = Enum.GetValues(typeof(KeyCode));
    
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
            Button keyButton = binding.transform.Find("Button").GetComponent<Button>();
            keyButton.onClick.AddListener(() => StartRebindFor(kvp.Key));
        }
    }

    private void Update()
    {
        if (actionToRebind != null)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in kcs)
                {
                    if (Input.GetKeyDown(kc))
                    {
                         SetKeyForAction(actionToRebind,kc);
                         actionToRebind = null;
                         break;
                    }
                }
            }
        }
    }

    void StartRebindFor(string action)
    {
        actionToRebind = action;
    }

    void SetKeyForAction(string action, KeyCode key)
    {
        Debug.Log(action + " should now be bound to " + key.ToString());
        actionKeys[action] = key;
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

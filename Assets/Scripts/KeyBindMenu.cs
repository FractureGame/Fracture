using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.UI;
public class KeyBindMenu : MonoBehaviour
{
    private InputManager inputManager;
    public GameObject keybindingPrefab;
    private string actionToRebind;
    private Dictionary<string, Text> buttonToLabel;
    Array kcs = Enum.GetValues(typeof(KeyCode));
    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        buttonToLabel = new Dictionary<string, Text>();
        GameObject parent = GameObject.Find("Keybinding Menu");
        foreach (var kvp in inputManager.actionKeys)
        {
            GameObject binding = Instantiate(keybindingPrefab, parent.transform);
            binding.transform.Find("Action Text").GetComponent<Text>().text = kvp.Key;
            Text buttonText = binding.transform.Find("Button/Key Text").GetComponent<Text>();
            buttonText.text = kvp.Value.ToString();
            buttonToLabel[kvp.Key] = buttonText;
            Button keyButton = binding.transform.Find("Button").GetComponent<Button>();
            keyButton.onClick.AddListener(() => StartRebindFor(kvp.Key));
        }
    }

    // Update is called once per frame
    void Update()
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
    void SetKeyForAction(string action, KeyCode key)
    {
        Debug.Log(action + " should now be bound to " + key.ToString());
        inputManager.actionKeys[action] = key;
        buttonToLabel[action].text = key.ToString();
    }
    void StartRebindFor(string action)
    {
        actionToRebind = action;
    }

    public void Serialize()
    {
        if (!Directory.Exists("FractureConfig"))
        {
            Directory.CreateDirectory("FractureConfig");
        }

        using (StreamWriter sw = new StreamWriter("FractureConfig/keyconfig.txt", false))
        { 
            foreach (var kvp in inputManager.actionKeys) 
            { 
                sw.WriteLine(kvp.Key + ":" + kvp.Value);
            }
        }
    }
    public static Dictionary<string,KeyCode> Deserialize(string path)
    {
        Dictionary<string, KeyCode> res = new Dictionary<string, KeyCode>();
        using (StreamReader sr = new StreamReader(path))
        {
            string l;
            while ((l = sr.ReadLine()) != null)
            {
                string[] actionKey = l.Split(':');
                Enum.TryParse(actionKey[1], out KeyCode key);
                res.Add(actionKey[0], key);
            }
        }
        return res;
    }
}

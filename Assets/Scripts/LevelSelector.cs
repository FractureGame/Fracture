using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Photon.Pun;

public class LevelSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buttonPrefab;
    public GameObject parent;
    public float spacing;
    public float buttonScale;
    public float beginningSpacing = 75;
    public void Start()
    {
        if (this.gameObject.name == "GameOverPanel")
        {
            Debug.Log("In game");
            Debug.Log(PhotonNetwork.PlayerList.Length);
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1)
            {
                Debug.Log("Is master, we display buttons");
                GameObject.Find("LevelButtons").SetActive(true);
                CreateButtons();
            }
            else
            {
                Debug.Log("Isnt master, displaying text");
                if (PhotonNetwork.PlayerList.Length > 1)
                {
                    GameObject.Find("WaitingMaster").SetActive(true);
                }
                else
                {
                    GameObject.Find("WaitingMaster").SetActive(false);
                }
                
            }
        }
        else
        {
            Debug.Log("In menus");
            CreateButtons();
        }
    }

    void CreateButtons()
    {
        parent.SetActive(true);
        int? toSkip = null;
        if (this.gameObject.name == "GameOverPanel")
        {
            toSkip = SceneManager.GetActiveScene().buildIndex;
        }
        int i = 0;
        foreach (var kvp in Levels.scenes)
        {
            if (toSkip != null && kvp.Value == toSkip)
            {
                continue;
            }
            GameObject button2 = Instantiate(buttonPrefab, parent.transform) as GameObject;
            button2.name = kvp.Key + " Button";
            button2.GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key;
            button2.GetComponent<LevelButton>().buildIndex = kvp.Value;
            Vector3 pos = button2.transform.position;
            Vector3 scale = button2.transform.localScale;
            /*scale.x = scale.x * buttonScale;
            scale.y = scale.y * buttonScale;
            scale.z = scale.z * buttonScale;*/
            pos.y -= i * spacing + beginningSpacing;
            button2.transform.position = pos;
            i++;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 1)
        {
            GameObject.Find("LevelButtons").SetActive(false);
            GameObject.Find("WaitingMaster").SetActive(false);
        }
    }
}

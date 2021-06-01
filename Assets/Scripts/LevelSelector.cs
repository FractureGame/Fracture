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
    public int spacing;
    public float buttonScale;
    public void Start()
    {
        if (this.gameObject.name == "GameOverPanel")
        {
            Debug.Log("In game");
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Is master, we display buttons");
                GameObject.Find("LevelButtons").SetActive(true);
                CreateButtons();
            }
            else
            {
                Debug.Log("Isnt master, displaying text");
                GameObject.Find("WaitingMaster").SetActive(true);
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
            if (toSkip != null && kvp.Value == i)
            {
                continue;
            }
            GameObject button2 = Instantiate(buttonPrefab, parent.transform) as GameObject;
            button2.name = kvp.Key + " Button";
            button2.GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key;
            button2.GetComponent<LevelButton>().buildIndex = kvp.Value;
            Vector3 pos = button2.transform.position;
            Vector3 scale = button2.transform.localScale;
            scale.x = scale.x * buttonScale;
            scale.y = scale.y * buttonScale;
            scale.z = scale.z * buttonScale;
            pos.y -= i * spacing;
            button2.transform.position = pos;
            i++;
        }
    }
    // Update is called once per frame
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject buttonPrefab;
    public Canvas canvas;
    public void Start()
    {
        int i = 0;
        foreach (KeyValuePair<string,string> kvp in Levels.scenes)
        {
            
            GameObject button2 = Instantiate(buttonPrefab,canvas.transform) as GameObject;
            button2.name = kvp.Key + " Button";
            Scene scene = SceneManager.GetSceneByPath(kvp.Value);
            if (!scene.IsValid())
            {
                Debug.Log("WARNING SCENE IS NULL MDRR");
            }
            button2.GetComponentInChildren<TextMeshProUGUI>().text = kvp.Key;
            button2.GetComponent<LevelButton>().pathToScene = kvp.Value;
            Vector3 pos = button2.transform.position;
            pos.y -= i * 75;
            button2.transform.position = pos;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

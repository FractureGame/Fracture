using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Start is called before the first frame update
    public int[] levels;
    public GameObject buttonPrefab;
    public Canvas canvas;
    public void Start()
    {
        int i = 0;
        foreach (int level in levels)
        {
            GameObject button2 = Instantiate(buttonPrefab,canvas.transform) as GameObject;
            Scene scene = SceneManager.GetSceneByBuildIndex(level);
            if (!scene.IsValid())
            {
                Debug.Log("WARNING SCENE IS NULL MDRR");
            }
            
            Debug.Log(scene.name);
            button2.GetComponentInChildren<TextMeshProUGUI>().text = scene.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

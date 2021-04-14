using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int buildIndex;

    public void LoadLevel()
    {
        SceneManager.LoadScene(buildIndex);
    }
}

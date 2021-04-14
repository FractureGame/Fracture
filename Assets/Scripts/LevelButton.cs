using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public string pathToScene;

    private void LoadLevel()
    {
        SceneManager.LoadScene(pathToScene);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbiesMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public void gameStart()
    {
        SceneManager.LoadScene("MainShit");
    }

    public void quitGame()
    {
        // Works in a built game
        Application.Quit();

        // For debugging in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

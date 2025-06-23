using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void GoToScene(int sceneIndex) {
        Debug.Log(sceneIndex);
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Application has quit");
    }


}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ContinueGame()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
    public void NewGame()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode(); 
        #endif
    }
}

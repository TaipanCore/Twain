using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void OnEnable()
    {
        String defaultGameSavePath = Path.Combine(Application.persistentDataPath, "Saves", "DefaultGameSave.json");
        G.isDefaultGameSaveExists = File.Exists(defaultGameSavePath);
        if (!G.isDefaultGameSaveExists)
            transform.Find("ContinueBtn").gameObject.SetActive(false);
    }
    
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

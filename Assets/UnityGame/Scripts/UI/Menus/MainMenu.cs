using System;
using System.IO;
using DG.Tweening;
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
        Time.timeScale = 1;
    }
    
    public void ContinueGame()
    {
        DOTween.KillAll();
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
    public void NewGame(bool isConfirmed)
    {
        if (!G.isDefaultGameSaveExists)
            isConfirmed = true;
        if (isConfirmed)
        {
            DOTween.KillAll();
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        }
        else
        {
            transform.Find("ConfirmationDialog").gameObject.SetActive(true);
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode(); 
        #endif
    }
}

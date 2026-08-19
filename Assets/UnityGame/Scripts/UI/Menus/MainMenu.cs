using System;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        G.audio.PlayMusic(G.music.menusMusic);
    }
    private void OnEnable()
    {
        String defaultGameSavePath = Path.Combine(Application.persistentDataPath, "Saves", "DefaultGameSave.json");
        G.isDefaultGameSaveExists = File.Exists(defaultGameSavePath);
        if (!G.isDefaultGameSaveExists)
            transform.Find("ContinueBtn").gameObject.SetActive(false);
        #if UNITY_WEBGL
            transform.Find("QuitGameBtn").gameObject.SetActive(false);
        #endif
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
            G.isDefaultGameSaveExists = false;
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

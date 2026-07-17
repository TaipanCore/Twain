using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    [SerializeField] private GameObject pauseElements;

    private void Awake()
    {
        G.gamePause = this;
    }
    private void Update()
    {
        if (G.input.HUDPauseBtnDown && (G.HUD.state == HUD.State.Game || G.HUD.state == HUD.State.Pause))
        {
            if (!pauseElements.activeInHierarchy)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        G.HUD.state = HUD.State.Pause;
        G.input.canPlayerInput = false;
        Time.timeScale = 0f;
        pauseElements.SetActive(true);
    }

    public void Resume()
    {
        G.HUD.state = HUD.State.Game;
        G.input.canPlayerInput = true;
        Time.timeScale = 1f;
        pauseElements.SetActive(false);
    }

    public void LoadMainMenu()
    {
        G.gameSaveLoad.SaveGame();
        DOTween.KillAll();
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}

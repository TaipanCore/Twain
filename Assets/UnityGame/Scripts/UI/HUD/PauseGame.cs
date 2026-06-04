using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject pauseElements;

    private void Update()
    {
        if (InputManager.HUDPauseBtnDown)
        {
            if (!pauseElements.activeInHierarchy)
            {
                InputManager.canPlayerInput = false;
                Time.timeScale = 0f;
                pauseElements.SetActive(true);
            }
            else
            {
                InputManager.canPlayerInput = true;
                Time.timeScale = 1f;
                pauseElements.SetActive(false);
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}

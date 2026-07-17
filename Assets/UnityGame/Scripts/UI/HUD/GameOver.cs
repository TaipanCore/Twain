using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverElements;

    private void Awake()
    {
        G.gameOver = this;
    }

    public void EndGame()
    {
        G.HUD.state = HUD.State.GameOver;
        G.input.canPlayerInput = false;
        Cursor.visible = true;
        Time.timeScale = 0f;
        gameOverElements.SetActive(true);
    }

    public void ResumeGame()
    {
        G.HUD.state = HUD.State.Game;
        G.input.canPlayerInput = true;
        Cursor.visible = false;
        Time.timeScale = 1f;
        gameOverElements.SetActive(false);
    }
}

using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverElements;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip playerRespawnSound;

    private void Awake()
    {
        G.gameOver = this;
    }

    public void EndGame()
    {
        G.HUD.state = HUD.State.GameOver;
        G.input.canPlayerInput = false;
        G.audio.PlaySoundEffect(gameOverSound);
        Cursor.visible = true;
        Time.timeScale = 0f;
        gameOverElements.SetActive(true);
    }

    public void ResumeGame()
    {
        G.HUD.state = HUD.State.Game;
        G.input.canPlayerInput = true;
        G.audio.PlaySoundEffect(playerRespawnSound);
        Cursor.visible = false;
        Time.timeScale = 1f;
        gameOverElements.SetActive(false);
    }
}

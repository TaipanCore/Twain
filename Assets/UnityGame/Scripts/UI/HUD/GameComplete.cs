using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameComplete : MonoBehaviour
{
    [SerializeField] private GameObject gameCompleteElements;
    [SerializeField] private float timeForReturnAvailable;

    private bool canReturnToMenu;
    
    private void Awake()
    {
        G.gameComplete = this;
    }
    
    public void EndGame()
    {
        G.HUD.state = HUD.State.GameComplete;
        G.input.canPlayerInput = false;
        gameCompleteElements.SetActive(true);
        Image background = gameCompleteElements.transform.Find("Background").GetComponent<Image>();
        TMP_Text congratulationsText = gameCompleteElements.transform.Find("CongratulationsText").GetComponent<TMP_Text>();
        TMP_Text returnToTheMainMenuPopup = gameCompleteElements.transform.Find("ReturnToMainMenuPopup").GetComponent<TMP_Text>();
        Sequence seq = DOTween.Sequence();
        seq
            .Append(background.DOFade(1f, 3f).SetEase(Ease.InCubic))
            .AppendCallback(() => G.audio.PauseAll())
            .AppendCallback(() => G.audio.PlayMusic(G.music.gameCompleteMusic))
            .Append(background.DOColor(Color.black, 2f).SetEase(Ease.OutQuint))
            .Append(congratulationsText.DOFade(1f, 0.5f))
            .AppendInterval(timeForReturnAvailable)
            .AppendCallback(() => canReturnToMenu = true);
        seq.OnComplete(() => returnToTheMainMenuPopup.DOFade(1f, 1f).SetLoops(-1, LoopType.Yoyo));
    }
    private void Update()
    {
        if (canReturnToMenu && G.input.HUDAnyKeyDown)
        {
            canReturnToMenu = false;
            G.input.canPlayerInput = true;
            DOTween.KillAll();
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}

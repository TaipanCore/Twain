using DG.Tweening;
using UnityEngine;

public class MapOpenerAndCloser : MonoBehaviour
{
    [SerializeField] private AudioClip mapOpenSound;
    
    [Header("Hints")]
    [SerializeField] private ButtonsHints buttonHints;
    public HintsTrigger hintsTrigger;
    
    private Camera mainCamera;
    private Camera mapCamera;
    private Canvas HUDCanvas;
    private bool isMapOpen;
    private Tween expandTween;

    private void Start()
    {
        mainCamera = G.mainCamera.GetComponent<Camera>();
        mapCamera = G.mapCamera.GetComponent<Camera>();
        HUDCanvas = GameObject.Find("HUD").GetComponent<Canvas>();
        hintsTrigger.Initialize
        (
            () => buttonHints.ShowHint(ButtonsHints.BtnKey.M),
            () => buttonHints.HideHint(ButtonsHints.BtnKey.M)
        );
    }

    private void Update()
    {
        if (G.input.mapBtnDown)
        {
            if (!isMapOpen && hintsTrigger.charactersInTrigger.Contains(G.characters.currentCharacter))
                OpenMap(G.characters.currentCharacter.transform.position);
            else if (isMapOpen)
                CloseMap();
        }
    }

    private void OpenMap(Vector3 openPosition)
    {
        G.HUD.state = HUD.State.Map;
        hintsTrigger.btnFirstActivated = true;
        G.mapCamera.SetActive(true);
        HUDCanvas.worldCamera = mapCamera;
        G.mainCamera.SetActive(false);
        mapCamera.transform.position = new Vector3(openPosition.x, openPosition.y, mapCamera.transform.position.z);
        G.audio.PlaySoundEffect(mapOpenSound, pitchMin: 0.65f, pitchMax: 0.75f);
        expandTween = mapCamera.DOOrthoSize(30f, 3f).SetEase(Ease.OutCubic).SetUpdate(true);
        expandTween.OnUpdate(() =>
        {
            if (G.input.mouseWheel != 0)
                expandTween.Kill();
        });
        isMapOpen = true;
        Time.timeScale = 0f;
    }
    
    private void CloseMap()
    {
        G.HUD.state = HUD.State.Game;
        G.mainCamera.SetActive(true);
        HUDCanvas.worldCamera = mainCamera;
        if (expandTween.IsActive())
            expandTween.Kill();
        mapCamera.orthographicSize = mainCamera.orthographicSize;
        G.mapCamera.SetActive(false);
        isMapOpen = false;
        Time.timeScale = 1f;
    }
}

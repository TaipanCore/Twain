using DG.Tweening;
using UnityEngine;

public class MapOpenerAndCloser : MonoBehaviour
{
    [SerializeField] private AudioClip mapOpenSound;
    
    private Camera mainCamera;
    private Camera mapCamera;
    private Canvas HUDCanvas;
    private bool isMapOpen;
    private bool canOpenMap;
    private Tween expandTween;

    private void Start()
    {
        mainCamera = G.mainCamera.GetComponent<Camera>();
        mapCamera = G.mapCamera.GetComponent<Camera>();
        HUDCanvas = GameObject.Find("HUD").GetComponent<Canvas>();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            canOpenMap = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            canOpenMap = false;
    }

    private void Update()
    {
        if (G.input.mapBtnDown)
        {
            if (!isMapOpen && canOpenMap)
                OpenMap(G.characters.currentCharacter.transform.position);
            else if (isMapOpen)
                CloseMap();
        }
    }

    private void OpenMap(Vector3 openPosition)
    {
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

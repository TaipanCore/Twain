using DG.Tweening;
using UnityEngine;

public class MapOpenerAndCloser : MonoBehaviour
{
    private Camera mainCamera;
    private Camera mapCamera;
    private bool isMapOpen;
    private Tween expandTween;

    private void Start()
    {
        mainCamera = GameManager.mainCamera.GetComponent<Camera>();
        mapCamera = GameManager.mapCamera.GetComponent<Camera>();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && InputManager.mapBtnDown)
        {
            if (!isMapOpen)
                OpenMap(other.transform.position);
            else
                CloseMap();
        }
    }

    private void OpenMap(Vector3 openPosition)
    {
        GameManager.mapCamera.SetActive(true);
        GameManager.mainCamera.SetActive(false);
        mapCamera.transform.position = new Vector3(openPosition.x, openPosition.y, mapCamera.transform.position.z);
        expandTween = mapCamera.DOOrthoSize(30f, 3f).SetEase(Ease.OutCubic);
        expandTween.OnUpdate(() =>
        {
            if (InputManager.mouseWheel != 0)
                expandTween.Kill();
        });
        isMapOpen = true;
    }
    
    private void CloseMap()
    {
        GameManager.mainCamera.SetActive(true);
        if (expandTween.IsActive())
            expandTween.Kill();
        mapCamera.orthographicSize = mainCamera.orthographicSize;
        GameManager.mapCamera.SetActive(false);
        isMapOpen = false;
    }
}

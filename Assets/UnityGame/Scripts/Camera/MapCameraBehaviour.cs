using DG.Tweening;
using UnityEngine;

public class MapCameraBehaviour : MonoBehaviour
{
    [SerializeField] private float maxCameraSize;
    [SerializeField] private float cameraZoomSpeed;
    
    private Bounds mapBounds;
    private Vector3 currentMousePos;
    private Vector3 previousMousePos;
    private Camera mapCamera;
    private Camera mainCamera;
    
    private void Awake()
    {
        GameManager.mapCamera = gameObject;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        mapBounds = GameObject.Find("Darkness").GetComponent<SpriteRenderer>().bounds;
        mapCamera = GameManager.mapCamera.GetComponent<Camera>();
        mainCamera = GameManager.mainCamera.GetComponent<Camera>();
    }

    private void Update()
    {
        if (InputManager.leftMouseBtnDown)
        {
            previousMousePos = MouseTracker.mousePosition;
        }
        if (InputManager.leftMouseBtn)
        {
            currentMousePos = MouseTracker.mousePosition;
            Vector3 movement = previousMousePos - currentMousePos;
            float clampedX = Mathf.Clamp(transform.position.x + movement.x, mapBounds.min.x, mapBounds.max.x);
            float clampedY = Mathf.Clamp(transform.position.y + movement.y, mapBounds.min.y, mapBounds.max.y);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
            previousMousePos = MouseTracker.mousePosition;
        }
        if (InputManager.mouseWheel != 0f)
        {
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize - InputManager.mouseWheel * cameraZoomSpeed, mainCamera.orthographicSize, maxCameraSize);
        }
    }
}

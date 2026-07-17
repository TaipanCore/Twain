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
        G.mapCamera = gameObject;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        mapBounds = GameObject.Find("Darkness").GetComponent<SpriteRenderer>().bounds;
        mapCamera = G.mapCamera.GetComponent<Camera>();
        mainCamera = G.mainCamera.GetComponent<Camera>();
    }

    private void Update()
    {
        if (G.input.leftMouseBtnDown)
        {
            previousMousePos = G.mouseTracker.mousePosition;
        }
        if (G.input.leftMouseBtn)
        {
            currentMousePos = G.mouseTracker.mousePosition;
            Vector3 movement = previousMousePos - currentMousePos;
            float clampedX = Mathf.Clamp(transform.position.x + movement.x, mapBounds.min.x, mapBounds.max.x);
            float clampedY = Mathf.Clamp(transform.position.y + movement.y, mapBounds.min.y, mapBounds.max.y);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
            previousMousePos = G.mouseTracker.mousePosition;
        }
        if (G.input.mouseWheel != 0f)
        {
            mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize - G.input.mouseWheel * cameraZoomSpeed, mainCamera.orthographicSize, maxCameraSize);
        }
    }
}

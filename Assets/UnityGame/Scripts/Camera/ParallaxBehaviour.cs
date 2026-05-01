using UnityEngine;

public class ParallaxBehaviour : MonoBehaviour
{
    [SerializeField] private float parallaxStrength;
    [SerializeField] private bool enableVerticalParallax;
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    
    private void Start()
    {
        cameraTransform = GameManager.mainCamera.GetComponent<Transform>();
        lastCameraPosition = cameraTransform.position;
    }
    private void Update()
    {
        Vector3 cameraMovement = cameraTransform.position - lastCameraPosition;
        if (!enableVerticalParallax)
            cameraMovement.y = 0f;
        transform.position += cameraMovement * (1 - parallaxStrength);
        lastCameraPosition = cameraTransform.position;
    }
}
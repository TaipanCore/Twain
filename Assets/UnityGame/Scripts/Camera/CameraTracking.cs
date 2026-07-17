using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private float interpolationMultiplier;

    private Transform cameraPoint;

    private void Awake()
    {
        G.mainCamera = gameObject;
    }
    private void Start()
    {
        cameraPoint = GetComponent<Transform>().parent;
        cameraPoint.position = G.characters.equilibrium.transform.position;
    }
    private void FixedUpdate()
    {
        if (G.characters.currentCharacter)
        {
            Vector3 target = G.characters.currentCharacter.transform.position;
            cameraPoint.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, -10), Time.fixedDeltaTime * interpolationMultiplier);
        }
    }
}

using DG.Tweening;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private float interpolationMultiplier;

    private Transform cameraPoint;

    private void Awake()
    {
        GameManager.mainCamera = gameObject;
    }
    private void Start()
    {
        cameraPoint = GetComponent<Transform>().parent;
        cameraPoint.position = GameManager.equilibrium.transform.position;
    }
    private void FixedUpdate()
    {
        if (GameManager.currentCharacter)
        {
            Vector3 target = GameManager.currentCharacter.transform.position;
            cameraPoint.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, -10), Time.fixedDeltaTime * interpolationMultiplier);
        }
    }
    public void ShakeCamera(float duration, float strength)
    {
        transform.DOShakePosition(duration, strength);
    }
}

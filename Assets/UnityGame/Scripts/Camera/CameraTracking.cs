using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private float interpolationMultiplier;

    private void Start()
    {
        transform.position = GameManager.Equilibrium.transform.position;
    }
    private void FixedUpdate()
    {
        if (GameManager.currentCharacter)
        {
            Vector3 target = GameManager.currentCharacter.transform.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, -10), Time.fixedDeltaTime * interpolationMultiplier);
        }
    }
}

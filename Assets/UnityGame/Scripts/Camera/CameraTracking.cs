using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    [SerializeField] private float interpolationMultiplier;

    private void FixedUpdate()
    {
        if (GameManager.currentCharacter != null)
        {
            Vector3 target = GameManager.currentCharacter.transform.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, -10), Time.fixedDeltaTime * interpolationMultiplier);
        }
    }
}

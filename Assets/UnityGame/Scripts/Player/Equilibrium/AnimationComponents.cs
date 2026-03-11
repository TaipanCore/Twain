using UnityEngine;

public class AnimationComponents : MonoBehaviour
{
    private delegate void VoidDelegate();
    
    private VoidDelegate shakeCamera;
    private VoidDelegate spawnBullet;

    private void Start()
    {
        shakeCamera = transform.parent.GetComponent<EquilibriumBehaviour>().ShakeCameraFromSteps;
        spawnBullet = transform.parent.GetComponent<EquilibriumBehaviour>().SpawnBullet;
    }

    private void ShakeCameraFromSteps()
    {
        shakeCamera?.Invoke();
    }

    private void SpawnBullet()
    {
        spawnBullet?.Invoke();
    }
}

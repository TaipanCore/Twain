using UnityEngine;

public class TorchBehaviour : MonoBehaviour
{
    protected GameObject lightSource;

    protected virtual void Awake()
    {
        lightSource = transform.GetChild(0).gameObject;
        GetComponent<TorchSounds>().PlayTorchBurningSound(lightSource.transform.position);
    }
}

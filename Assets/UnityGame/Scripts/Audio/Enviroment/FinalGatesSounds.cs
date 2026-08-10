using UnityEngine;

public class FinalGatesSounds : MonoBehaviour
{
    [SerializeField] private AudioClip chargingSound;

    public AudioSource PlayChargingSound()
    {
        return G.audio.PlaySoundEffectAtPoint(chargingSound, transform.position, loop: true);
    }
}

using UnityEngine;

public class ShrineOfBalanceSounds : MonoBehaviour
{
    [SerializeField] private AudioClip chargedSound;
    [SerializeField] private AudioClip getChargeSound;

    public void PlayChargedSound()
    {
        G.audio.PlaySoundEffectAtPoint(chargedSound, transform.position);
    }
    public void PlayGetChargeSound()
    {
        G.audio.PlaySoundEffectAtPoint(getChargeSound, transform.position);
    }
}

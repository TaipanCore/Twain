using UnityEngine;

public class LightSideSounds : MonoBehaviour
{
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private AudioClip damagedSound;

    public void PlayStepSound()
    {
        G.audio.PlaySoundEffectAtPoint(stepSound, transform.position, pitchMin: 0.85f,  pitchMax: 1.15f);
    }

    public void PlayDamagedSound()
    {
        G.audio.PlaySoundEffectAtPoint(damagedSound,  transform.position);
    }
    
}

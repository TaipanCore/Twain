using UnityEngine;

public class EquilibriumSounds : MonoBehaviour
{
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hookEjectSound;

    public void PlayStepSound()
    {
        G.audio.PlaySoundEffectAtPoint(stepSound, transform.position, pitchMin: 0.8f, pitchMax: 1f);
    }

    public void PlayShootSound()
    {
        G.audio.PlaySoundEffectAtPoint(shootSound, transform.position);
    }

    public void PlayAttackSound()
    {
        G.audio.PlaySoundEffectAtPoint(attackSound,  transform.position, pitchMin: 0.5f, pitchMax: 0.5f);
    }

    public void PlayHookEjectSound()
    {
        G.audio.PlaySoundEffectAtPoint(hookEjectSound, transform.position, pitchMin: 0.8f, pitchMax: 0.9f);
    }
}

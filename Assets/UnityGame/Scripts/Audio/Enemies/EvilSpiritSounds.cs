using UnityEngine;

public class EvilSpiritSounds : MonoBehaviour
{
    [SerializeField] private AudioClip aggroSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip inFocusedLightSound;
    [SerializeField] private AudioClip dieSound;
    
    public void PlayAggroSound()
    {
        G.audio.PlaySoundEffectAtPoint(aggroSound, transform.position);
    }
    public void PlayAttackSound()
    {
        G.audio.PlaySoundEffectAtPoint(attackSound, transform.position, pitchMin: 0.9f, pitchMax: 1.1f);
    }
    public void PlayDashSound()
    {
        G.audio.PlaySoundEffectAtPoint(dashSound, transform.position, pitchMin: 1f, pitchMax: 1.3f);
    }

    public AudioSource PlayInFocusedLightSound()
    {
        return G.audio.PlaySoundEffectAtPoint(inFocusedLightSound, transform.position, pitchMin: 0.8f, pitchMax: 1.1f);
    }
    public void PlayDieSound()
    {
        G.audio.PlaySoundEffectAtPoint(dieSound, transform.position);
    }

}

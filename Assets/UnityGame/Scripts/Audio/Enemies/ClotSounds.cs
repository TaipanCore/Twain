using UnityEngine;

public class ClotSounds : MonoBehaviour
{
    [SerializeField] private AudioClip stepSound;
    [SerializeField] private AudioClip[] aggroSounds;
    [SerializeField] private AudioClip hookHitSound;
    [SerializeField] private AudioClip bulletHitSound;
    [SerializeField] private AudioClip stunnedSound;
    [SerializeField] private AudioClip dieSound;
    [SerializeField] private AudioClip etherEjectSound;

    public void PlayStepSound()
    {
        G.audio.PlaySoundEffectAtPoint(stepSound, transform.position, pitchMin: 0.8f, pitchMax: 1f);
    }
    public void PlayAggroSound()
    {
        G.audio.PlaySoundEffectAtPoint(aggroSounds[Random.Range(0, aggroSounds.Length)], transform.position);
    }
    public void PlayHookHitSound()
    {
        G.audio.PlaySoundEffectAtPoint(hookHitSound, transform.position);
    }
    public void PlayBulletHitSound()
    {
        G.audio.PlaySoundEffectAtPoint(bulletHitSound, transform.position, pitchMin: 0.8f, pitchMax: 0.8f);
        G.audio.PlaySoundEffectAtPoint(stunnedSound, transform.position);
    }
    public void PlayDieSound()
    {
        G.audio.PlaySoundEffectAtPoint(dieSound, transform.position);
    }
    public void PlayEtherEjectSound()
    {
        G.audio.PlaySoundEffectAtPoint(etherEjectSound, transform.position,  pitchMin: 0.7f, pitchMax: 1.2f);
    }

}

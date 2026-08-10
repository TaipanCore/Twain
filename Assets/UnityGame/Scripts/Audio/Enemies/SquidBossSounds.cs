using UnityEngine;

public class SquidBossSounds : MonoBehaviour
{
    [SerializeField] private AudioClip eyePopSound;
    [SerializeField] private AudioClip appearSound;
    [SerializeField] private AudioClip tentacleHitSound;
    [SerializeField] private AudioClip dieSound;

    public void PlayEyePopSound(Vector3 position, float pitch)
    {
        G.audio.PlaySoundEffectAtPoint(eyePopSound, position, pitchMin: pitch, pitchMax: pitch);
    }
    public void PlayAppearSound()
    {
        G.audio.PlaySoundEffectAtPoint(appearSound, transform.position, pitchMin: 0.8f, pitchMax: 1f);
    }
    public void PlayTentacleHitSound()
    {
        G.audio.PlaySoundEffectAtPoint(tentacleHitSound, transform.position, pitchMin: 0.6f, pitchMax: 0.75f);
    }
    public void PlayDieSound()
    {
        G.audio.PlaySoundEffectAtPoint(dieSound, transform.position);
    }
}

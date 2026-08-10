using UnityEngine;

public class FireflySounds : MonoBehaviour
{
    [SerializeField] private AudioClip fireflyLifeSound;

    public AudioSource PlayFireflyLifeSound()
    {
        return G.audio.PlaySoundEffectAtPoint(fireflyLifeSound, transform.position, pitchMin: 0.9f, pitchMax: 1.1f);
    }
}

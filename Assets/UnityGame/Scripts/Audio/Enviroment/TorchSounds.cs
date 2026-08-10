using UnityEngine;

public class TorchSounds : MonoBehaviour
{
    [SerializeField] private AudioClip torchFiringSound;
    [SerializeField] private AudioClip torchBurningSound;

    public AudioSource PlayTorchFiringSound(Vector3 position)
    {
        return G.audio.PlaySoundEffectAtPoint(torchFiringSound, position, pitchMin: 0.9f, pitchMax: 1.1f);
    }
    public void PlayTorchBurningSound(Vector3 position)
    {
        G.audio.PlaySoundEffectAtPoint(torchBurningSound, position, pitchMin: 0.95f, pitchMax: 1.05f, loop: true, time: Random.Range(0f, 2f));
    }
}

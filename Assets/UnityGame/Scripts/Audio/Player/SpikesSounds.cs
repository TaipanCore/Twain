using UnityEngine;

public class SpikesSounds : MonoBehaviour
{
    [SerializeField] private AudioClip spikeSpawnSound;
    [SerializeField] private AudioClip spikeDespawnSound;

    public void PlaySpikeSpawnSound(Vector3 position)
    {
        G.audio.PlaySoundEffectAtPoint(spikeSpawnSound, position, pitchMin: 0.9f, pitchMax: 1.1f);
    }

    public void PlaySpikeDespawnSound(Vector3 position)
    {
        G.audio.PlaySoundEffectAtPoint(spikeDespawnSound, position, pitchMin: 0.8f, pitchMax: 1f);
    }
}

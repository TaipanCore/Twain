using UnityEngine;

public class ShardSounds : MonoBehaviour
{
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private AudioClip moveToSlotSound;

    public void PlayPickUpSound()
    {
        G.audio.PlaySoundEffectAtPoint(pickUpSound, transform.position);
    }

    public void PlayMoveToSlotSound()
    {
        G.audio.PlaySoundEffectAtPoint(moveToSlotSound, transform.position);
    }
}

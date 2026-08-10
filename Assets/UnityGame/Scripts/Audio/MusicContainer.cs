using UnityEngine;

public class MusicContainer : MonoBehaviour
{
    [Header("Music")]
    public AudioClip menusMusic;
    public AudioClip labyrinthMusic;
    public AudioClip forestMusic;
    public AudioClip squidBossMusic;
    public AudioClip evilSpiritMusic;
    public AudioClip gameCompleteMusic;

    private void Awake()
    {
        G.music = this;
    }
}

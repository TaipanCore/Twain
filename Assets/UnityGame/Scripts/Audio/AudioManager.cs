using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundEffectsSource;
    
    private void Awake()
    {
        G.audio = this;
    }
    
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", GetVolumeInDB(volume));
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", GetVolumeInDB(volume));
    }
    public void SetSoundEffectsVolume(float volume)
    {
        audioMixer.SetFloat("SoundEffectsVolume", GetVolumeInDB(volume));
    }

    private float GetVolumeInDB(float volume)
    {
        float clampedVolume = Mathf.Clamp(volume, 0.001f, 1f);
        return Mathf.Log10(clampedVolume) * 20f;
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void PlaySoundEffect(AudioClip clip, float volume = 1f, float pitchMin = 1f, float pitchMax = 1f)
    {
        soundEffectsSource.pitch = Random.Range(pitchMin, pitchMax);
        soundEffectsSource.PlayOneShot(clip, volume);
    }
}

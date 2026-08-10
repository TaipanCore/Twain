using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup soundEffectsGroup;

    [SerializeField] private AudioSource audioSourcePrefab;
    private ObjectsPool<AudioSource> audioPool;
    
    public bool hasCurrentMusic => currentMusic;
    
    private AudioSource currentMusic;
    private Sequence musicSequence;
    
    private HashSet<AudioSource> pausedSources = new ();

    private void Awake()
    {
        G.audio = this;
        audioPool = new (10, CreateFunc);
    }
    
    private AudioSource CreateFunc() => Instantiate(audioSourcePrefab, transform);
    
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

    public void PauseAll()
    {
        pausedSources = new HashSet<AudioSource>(audioPool.GetActiveObjects());
        foreach (AudioSource audioSource in pausedSources)
            audioSource.Pause();
    }

    public void ResumeAll()
    {
        foreach (AudioSource audioSource in pausedSources)
            audioSource.UnPause();
        pausedSources.Clear();
    }

    public AudioSource PlayMusic(AudioClip clip, float volume = 1f, bool loop = true, float time = 0f, float fadeDuration = 3f)
    {
        currentMusic ??= audioPool.Get();
        currentMusic.outputAudioMixerGroup = musicGroup;
        currentMusic.spatialBlend = 0f;
        musicSequence?.Kill();
        musicSequence = DOTween.Sequence();
        if (currentMusic.isPlaying)
            musicSequence.Append(DOVirtual.Float(volume, 0f, fadeDuration, value => currentMusic.volume = value));
        musicSequence
            .AppendCallback(() =>
            {
                currentMusic.clip = clip;
                currentMusic.loop = loop;
                currentMusic.time = time;
            })
            .AppendCallback(() => currentMusic.Play())
            .Append(DOVirtual.Float(0f, volume, fadeDuration, value => currentMusic.volume = value));
        if (!loop)
            StartCoroutine(WaitForSoundEnd(currentMusic));
        return currentMusic;
    }
    
    public void StopMusic() => currentMusic?.Stop();

    public AudioSource PlaySoundEffect(AudioClip clip, float volume = 1f, float pitchMin = 1f, float pitchMax = 1f, bool loop = false, float time = 0f)
    {
        AudioSource soundEffectsSource = audioPool.Get();
        soundEffectsSource.outputAudioMixerGroup = soundEffectsGroup;
        soundEffectsSource.loop = loop;
        soundEffectsSource.time = time;
        soundEffectsSource.spatialBlend = 0f;
        soundEffectsSource.pitch = Random.Range(pitchMin, pitchMax);
        soundEffectsSource.clip = clip;
        soundEffectsSource.volume = volume;
        soundEffectsSource.Play();
        if (!loop)
            StartCoroutine(WaitForSoundEnd(soundEffectsSource));
        return soundEffectsSource;
    }

    public AudioSource PlaySoundEffectAtPoint(AudioClip clip, Vector3 position, float volume = 1f, float pitchMin = 1f, float pitchMax = 1f, bool loop = false, float time = 0f)
    {
        AudioSource soundEffectsSource = audioPool.Get();
        soundEffectsSource.outputAudioMixerGroup = soundEffectsGroup;
        soundEffectsSource.loop = loop;
        soundEffectsSource.time = time;
        soundEffectsSource.gameObject.transform.position = position;
        soundEffectsSource.spatialBlend = 1f;
        soundEffectsSource.pitch = Random.Range(pitchMin, pitchMax);
        soundEffectsSource.clip = clip;
        soundEffectsSource.volume = volume;
        soundEffectsSource.Play();
        if (!loop)
            StartCoroutine(WaitForSoundEnd(soundEffectsSource));
        return soundEffectsSource;
    }

    private IEnumerator WaitForSoundEnd(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying || pausedSources.Contains(source));
        audioPool.Return(source);
    }
}

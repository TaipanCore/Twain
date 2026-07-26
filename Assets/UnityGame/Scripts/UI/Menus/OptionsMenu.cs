using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Locale = UnityEngine.Localization.Locale;
using Slider = UnityEngine.UI.Slider;

public class OptionsMenu : MonoBehaviour
{
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider soundEffectsVolumeSlider;
    
    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        String languageCode = PlayerPrefs.GetString("LanguageCode", "en");
        SetLanguage(languageCode);
        String transformPath = languageCode == "ru" ? "LanguageOption/RussianLanguage" : "LanguageOption/EnglishLanguage";
        transform.Find(transformPath).GetComponent<Toggle>().isOn = true;
        
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 5);
        masterVolumeSlider = transform.Find("Audio/MasterVolume").GetComponent<Slider>();
        masterVolumeSlider.SetValueWithoutNotify(masterVolume);
        SetMasterVolume(masterVolume);

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 5);
        musicVolumeSlider = transform.Find("Audio/MusicVolume").GetComponent<Slider>();
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        SetMusicVolume(musicVolume);

        float soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 5);
        soundEffectsVolumeSlider = transform.Find("Audio/SoundEffectsVolume").GetComponent<Slider>();
        soundEffectsVolumeSlider.SetValueWithoutNotify(soundEffectsVolume);
        SetSoundEffectsVolume(soundEffectsVolume);

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        gameObject.SetActive(false);
    }

    public void SetLanguage(String languageCode)
    {
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        LocalizationSettings.SelectedLocale = locale;
        PlayerPrefs.SetString("LanguageCode", languageCode);
        PlayerPrefs.Save();
    }
    public void SetMasterVolume(float volume)
    {
        float normalizedVolume = volume / masterVolumeSlider.maxValue;
        G.audio.SetMasterVolume(normalizedVolume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
    public void SetMusicVolume(float volume)
    {
        float normalizedVolume = volume / musicVolumeSlider.maxValue;
        G.audio.SetMusicVolume(normalizedVolume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
    public void SetSoundEffectsVolume(float volume)
    {
        float normalizedVolume = volume / soundEffectsVolumeSlider.maxValue;
        G.audio.SetSoundEffectsVolume(normalizedVolume);
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        PlayerPrefs.Save();
    }
}

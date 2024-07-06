using UnityEngine;
using System;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // Define events for settings changes
    public event Action<float> OnSoundEffectsVolumeChanged;
    public event Action<float> OnBackgroundMusicVolumeChanged;

    // Define keys for PlayerPrefs
    private const string SOUND_EFFECTS_VOLUME_KEY = "SoundEffectsVolume";
    private const string BACKGROUND_MUSIC_VOLUME_KEY = "BackgroundMusicVolume";

    // Cache for current settings
    private float _soundEffectsVolume = 1f;
    private float _backgroundMusicVolume = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Sound Effects Volume
    public float GetSoundEffectsVolume()
    {
        return _soundEffectsVolume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        _soundEffectsVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SOUND_EFFECTS_VOLUME_KEY, _soundEffectsVolume);
        PlayerPrefs.Save();
        OnSoundEffectsVolumeChanged?.Invoke(_soundEffectsVolume);
    }

    // Background Music Volume
    public float GetBackgroundMusicVolume()
    {
        return _backgroundMusicVolume;
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        _backgroundMusicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(BACKGROUND_MUSIC_VOLUME_KEY, _backgroundMusicVolume);
        PlayerPrefs.Save();
        OnBackgroundMusicVolumeChanged?.Invoke(_backgroundMusicVolume);
        Debug.Log($"SettingsManager: Background Music Volume set to {_backgroundMusicVolume}");
    }

    // Load all settings
    private void LoadAllSettings()
    {
        _soundEffectsVolume = PlayerPrefs.GetFloat(SOUND_EFFECTS_VOLUME_KEY, 1f);
        _backgroundMusicVolume = PlayerPrefs.GetFloat(BACKGROUND_MUSIC_VOLUME_KEY, 1f);

        // Invoke events to notify listeners of initial values
        OnSoundEffectsVolumeChanged?.Invoke(_soundEffectsVolume);
        OnBackgroundMusicVolumeChanged?.Invoke(_backgroundMusicVolume);
    }

    // Reset all settings to default values
    public void ResetToDefaults()
    {
        SetSoundEffectsVolume(1f);
        SetBackgroundMusicVolume(1f);
    }
}
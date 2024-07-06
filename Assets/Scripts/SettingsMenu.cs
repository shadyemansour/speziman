using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider backgroundMusicSlider;
    [SerializeField] private Button backButton;

    private void Start()
    {
        // Ensure the SoundManager exists
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager not found in the scene!");
            return;
        }

        // Initialize slider values
        soundEffectsSlider.value = SoundManager.Instance.GetSoundEffectsVolume();
        backgroundMusicSlider.value = SoundManager.Instance.GetBackgroundMusicVolume();

        // Add listeners to sliders
        soundEffectsSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);
        backgroundMusicSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChanged);

        // Add listener to back button
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnSoundEffectsVolumeChanged(float volume)
    {
        SoundManager.Instance.SetSoundEffectsVolume(volume);
    }

    private void OnBackgroundMusicVolumeChanged(float volume)
    {
        SoundManager.Instance.SetBackgroundMusicVolume(volume);
    }

    private void OnBackButtonClicked()
    {
        Debug.Log("Back button clicked");
    }
}
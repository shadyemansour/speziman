using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider backgroundMusicSlider;
    [SerializeField] private Button backButton;
    private MenuController menuController;

    private void Start()
    {
        // Ensure the SoundManager exists
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager not found in the scene!");
            return;
        }

        // Initialize slider values
        soundEffectsSlider.value = SettingsManager.Instance.GetSoundEffectsVolume();
        backgroundMusicSlider.value = SettingsManager.Instance.GetBackgroundMusicVolume();

        // Add listeners to sliders
        soundEffectsSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);
        backgroundMusicSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChanged);

        menuController = FindObjectOfType<MenuController>();
        if (menuController == null)
        {
            Debug.LogError("MenuController not found in the scene!");
        }

        // Add listener to back button
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnSoundEffectsVolumeChanged(float volume)
    {
        SettingsManager.Instance.SetSoundEffectsVolume(volume);
    }

    private void OnBackgroundMusicVolumeChanged(float volume)
    {
        SettingsManager.Instance.SetBackgroundMusicVolume(volume);
    }

    private void OnBackButtonClicked()
    {
        if (menuController != null)
        {
            menuController.CloseSettingsMenu();
        }
        else
        {
            Debug.LogError("MenuController is null in SettingsMenu!");
        }
    }
}
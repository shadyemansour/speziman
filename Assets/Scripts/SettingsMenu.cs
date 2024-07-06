using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private Slider backgroundMusicSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private AudioClip testSoundEffect;

    private MenuController menuController;

    private void Start()
    {
        menuController = FindObjectOfType<MenuController>();
        if (menuController == null)
        {
            Debug.LogError("MenuController not found in the scene!");
        }

        // Initialize slider values
        soundEffectsSlider.value = SettingsManager.Instance.GetSoundEffectsVolume();
        backgroundMusicSlider.value = SettingsManager.Instance.GetBackgroundMusicVolume();

        // Add listeners to sliders
        soundEffectsSlider.onValueChanged.AddListener(OnSoundEffectsVolumeChanged);
        backgroundMusicSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChanged);

        // Add listener to back button
        backButton.onClick.AddListener(OnBackButtonClicked);

        // Subscribe to SettingsManager events
        SettingsManager.Instance.OnSoundEffectsVolumeChanged += UpdateSoundEffectsSlider;
        SettingsManager.Instance.OnBackgroundMusicVolumeChanged += UpdateBackgroundMusicSlider;
    }

    private void OnDestroy()
    {
        // Unsubscribe from SettingsManager events
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnSoundEffectsVolumeChanged -= UpdateSoundEffectsSlider;
            SettingsManager.Instance.OnBackgroundMusicVolumeChanged -= UpdateBackgroundMusicSlider;
        }
    }

    private void OnSoundEffectsVolumeChanged(float volume)
    {
        Debug.Log($"Sound Effects Volume Changed: {volume}");
        SettingsManager.Instance.SetSoundEffectsVolume(volume);
        SoundManager.Instance.PlaySound("testSound");
    }

    private void OnBackgroundMusicVolumeChanged(float volume)
    {
        Debug.Log($"Background Music Volume Changed: {volume}");
        SettingsManager.Instance.SetBackgroundMusicVolume(volume);
        // Directly update SoundManager to ensure immediate effect
        SoundManager.Instance.SetBackgroundMusicVolume(volume);
    }

    private void UpdateSoundEffectsSlider(float volume)
    {
        soundEffectsSlider.SetValueWithoutNotify(volume);
    }

    private void UpdateBackgroundMusicSlider(float volume)
    {
        backgroundMusicSlider.SetValueWithoutNotify(volume);
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

    private void PlayTestSound()
    {
        if (testSoundEffect != null)
        {
            SoundManager.Instance.PlaySound("testSound");
        }
    }
}
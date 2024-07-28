using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingsController : MonoBehaviour
{

    [SerializeField] private GameObject burgerButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private Animator pauseAnimator;
    private Animator settingsAnimator;
    private Animator backgroundAnimator;
    private CanvasGroup settingsCanvas;
    private CanvasGroup backgroundCanvas;
    private CanvasGroup pauseCanvas;
    private bool pauseOpen = false;
    private bool isStartMenu;
    private bool isCut;

    private static SettingsController instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        musicSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxSlider.value = SoundManager.Instance.GetSFXVolume();
        musicSlider.onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);

        settingsAnimator = settingsPanel.GetComponent<Animator>();
        pauseAnimator = pausePanel.GetComponent<Animator>();
        backgroundAnimator = backgroundPanel.GetComponent<Animator>();

        settingsCanvas = settingsPanel.GetComponent<CanvasGroup>();
        pauseCanvas = pausePanel.GetComponent<CanvasGroup>();
        backgroundCanvas = backgroundPanel.GetComponent<CanvasGroup>();

    }

    public void OpenSettings()
    {
        string sceneName = SceneManager.GetActiveScene().name.ToLower();
        isStartMenu = sceneName.Contains("start");
        isCut = sceneName.Contains("cut");
        if (isStartMenu)
        {
            settingsAnimator.Play("ZoomInAndFadeInSettings");
            backgroundAnimator.Play("ZoomInAndFadeInSettings");
            settingsCanvas.interactable = true;
            settingsCanvas.blocksRaycasts = true;
            backgroundCanvas.interactable = true;
            backgroundCanvas.blocksRaycasts = true;
            burgerButton.SetActive(false);


        }
        else
        {
            if (isCut)
            {
                FindAnyObjectByType<UIManager>().Pause();
            }
            else
            {
                GameManager.Instance.ToggleGamePause();
            }
            pauseAnimator.Play("ZoomInAndFadeInSettings");
            backgroundAnimator.Play("ZoomInAndFadeInSettings");
            pauseCanvas.interactable = true;
            pauseCanvas.blocksRaycasts = true;
            backgroundCanvas.interactable = true;
            backgroundCanvas.blocksRaycasts = true;
            burgerButton.SetActive(false);
            pauseOpen = true;
        }

    }

    public void CloseSettings(bool isRestart = false)
    {
        if (isStartMenu)
        {
            settingsAnimator.Play("ZoomOutFadeOutSettings");
            backgroundAnimator.Play("ZoomOutFadeOutSettings");
            settingsCanvas.interactable = false;
            settingsCanvas.blocksRaycasts = false;
            backgroundCanvas.interactable = false;
            backgroundCanvas.blocksRaycasts = false;
            burgerButton.SetActive(true);
        }
        else
        {
            if (pauseOpen)
            {
                pauseAnimator.Play("ZoomOutFadeOutSettings");
                pauseCanvas.interactable = false;
                pauseCanvas.blocksRaycasts = false;


                backgroundAnimator.Play("ZoomOutFadeOutSettings");
                backgroundCanvas.interactable = false;
                backgroundCanvas.blocksRaycasts = false;

                pauseOpen = false;
                burgerButton.SetActive(true);

                if (isRestart)
                {
                    SoundManager.Instance.StopSFX();
                    return;
                }
                if (isCut)
                {
                    FindAnyObjectByType<UIManager>().Resume();
                }
                else
                {
                    GameManager.Instance.ToggleGamePause();
                }
            }
            else
            {
                settingsAnimator.Play("ZoomOutFadeOutSettings");
                settingsCanvas.interactable = false;
                settingsCanvas.blocksRaycasts = false;

                pauseAnimator.Play("ZoomInAndFadeInSettings");
                pauseCanvas.interactable = true;
                pauseCanvas.blocksRaycasts = true;
                pauseOpen = true;

            }
        }
        EventSystem.current.SetSelectedGameObject(null);


    }

    public void GoToMenu()
    {
        CloseSettings();
        GameManager.Instance.LoadMenu();
    }

    public void RestartLevel()
    {
        CloseSettings(true);
        GameManager.Instance.RestartLevel(isCut);
    }

    public void OpenVolumeSettings()
    {
        pauseAnimator.Play("ZoomOutFadeOutSettings");
        pauseCanvas.interactable = false;
        pauseCanvas.blocksRaycasts = false;
        pauseOpen = false;

        settingsAnimator.Play("ZoomInAndFadeInSettings");
        settingsCanvas.interactable = true;
        settingsCanvas.blocksRaycasts = true;
    }
}

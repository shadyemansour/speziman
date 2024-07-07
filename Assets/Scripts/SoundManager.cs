using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private Dictionary<string, AudioClip> audioClips;
    private List<AudioClip> dieSounds;
    private AudioSource musicSource;
    private AudioSource sfxSource;


    private float soundEffectsVolume = 1f;
    private float backgroundMusicVolume = 1f;
    [SerializeField] private AudioClip testSoundEffect;
    [SerializeField] private AudioClip backgroundMusicClip;

    private void Start()
    {
        // Initialize volumes
        SetSoundEffectsVolume(SettingsManager.Instance.GetSoundEffectsVolume());
        SetBackgroundMusicVolume(SettingsManager.Instance.GetBackgroundMusicVolume());
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundManager();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeSoundManager()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources[0];
        sfxSource = sources[1];

        // Initialize volumes
        SetSoundEffectsVolume(SettingsManager.Instance.GetSoundEffectsVolume());
        SetBackgroundMusicVolume(SettingsManager.Instance.GetBackgroundMusicVolume());

        // Initialize audio clips dictionary and die sounds list
        InitializeAudioClips();

        // Subscribe to SettingsManager events
        SettingsManager.Instance.OnSoundEffectsVolumeChanged += SetSoundEffectsVolume;
        SettingsManager.Instance.OnBackgroundMusicVolumeChanged += SetBackgroundMusicVolume;

        // Start playing background music
        PlayBackgroundMusic(backgroundMusicClip);

        // Subscribe to scene loading event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure background music is playing when a new scene is loaded
        if (!musicSource.isPlaying)
        {
            PlayBackgroundMusic(backgroundMusicClip);
        }
    }

    private void InitializeAudioClips()
    {
        
        audioClips = new Dictionary<string, AudioClip>
        {
            { "jump", Resources.Load<AudioClip>("Audio/jump") },
            { "checkpoint", Resources.Load<AudioClip>("Audio/checkpoint") },
            { "levelComplete", Resources.Load<AudioClip>("Audio/levelComplete") },
            { "collectCola", Resources.Load<AudioClip>("Audio/Collect") },
            { "collectOrange", Resources.Load<AudioClip>("Audio/Collect_2") },
            { "collectSpezi", Resources.Load<AudioClip>("Audio/Collect") },
            { "pig", Resources.Load<AudioClip>("Audio/pig") },
            { "boost", Resources.Load<AudioClip>("Audio/boost") },
            { "levelFailed", Resources.Load<AudioClip>("Audio/levelFailed") },
            { "cut11", Resources.Load<AudioClip>("Audio/Narration/Cut1/1") },
            { "cut12", Resources.Load<AudioClip>("Audio/Narration/Cut1/2") },
            { "cut13", Resources.Load<AudioClip>("Audio/Narration/Cut1/3") },
            { "cut14", Resources.Load<AudioClip>("Audio/Narration/Cut1/4") },
            { "cut15", Resources.Load<AudioClip>("Audio/Narration/Cut1/5") },
            { "cut16", Resources.Load<AudioClip>("Audio/Narration/Cut1/6") },
            { "cut21", Resources.Load<AudioClip>("Audio/Narration/Cut2/1") },
            { "cut22", Resources.Load<AudioClip>("Audio/Narration/Cut2/2") },
            { "cut23", Resources.Load<AudioClip>("Audio/Narration/Cut2/3") },
            { "cut24", Resources.Load<AudioClip>("Audio/Narration/Cut2/4") },
            { "cut25", Resources.Load<AudioClip>("Audio/Narration/Cut2/5") },
            { "cut26", Resources.Load<AudioClip>("Audio/Narration/Cut2/6") },
            { "cut27", Resources.Load<AudioClip>("Audio/Narration/Cut2/7") },
        };

        // Load die sounds
        dieSounds = new List<AudioClip>();
        for (int i = 1; i <= 12; i++)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/Grumpy Bavarian Selection/{i}");
            if (clip != null)
            {
                dieSounds.Add(clip);
            }
            else
            {
                Debug.LogWarning($"Die sound {i}.mp3 not found");
            }
        }

        // Add test sound to the dictionary
        if (testSoundEffect != null)
        {
            audioClips["testSound"] = testSoundEffect;
        }
        else
        {
            Debug.LogWarning("Test sound effect is not assigned in SoundManager!");
        }
    }

        
        // AudioSource[] sources = GetComponents<AudioSource>();
        // musicSource = sources[0];
        // sfxSource = sources[1];



        


    

    private void OnDestroy()
    {
        // Unsubscribe from SettingsManager events
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnSoundEffectsVolumeChanged -= SetSoundEffectsVolume;
            SettingsManager.Instance.OnBackgroundMusicVolumeChanged -= SetBackgroundMusicVolume;
        }


        // Unsubscribe from scene loading event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public float PlaySound(string clipKey)
    {
        float audioLength = 0;
        if (audioClips.TryGetValue(clipKey, out AudioClip clip))
        {
            audioLength = clip.length;
            sfxSource.PlayOneShot(clip, soundEffectsVolume);
        }
        else
        {
            Debug.LogWarning("Sound clip not found: " + clipKey);
        }
        return audioLength;
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }   

        public void StopBackground()
    {
        musicSource.Stop();
    }   



    public void PlayRandomDieSound()
    {
        if (dieSounds.Count > 0)
        {
            int randomIndex = Random.Range(0, dieSounds.Count);
            sfxSource.PlayOneShot(dieSounds[randomIndex]);
        }
        else
        {
            Debug.LogWarning("No die sounds available");
        }
    }


    public void FadeOutBackgroundSound(float fadeOutTime = 2f)
    {
        StartCoroutine(FadeOut(musicSource, fadeOutTime));
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeOutTime)
    {
        float startVolume = backgroundMusicVolume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }


    public float GetSoundEffectsVolume()
    {
        return soundEffectsVolume;
    }

    public float GetBackgroundMusicVolume()
    {
        return backgroundMusicVolume;
    }

    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsVolume = volume;
        sfxSource.volume = soundEffectsVolume;
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        if (musicSource != null && musicSource.clip != null)
        {
            musicSource.volume = backgroundMusicVolume;
        }
        Debug.Log($"Background Music Volume set to: {volume}");
    }


public void PlayBackgroundMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicClip != null)
        {
            if (musicSource.clip != musicClip || !musicSource.isPlaying)
            {
                musicSource.clip = musicClip;
                musicSource.volume = backgroundMusicVolume;
                musicSource.loop = true;
                musicSource.Play();
                Debug.Log($"Playing background music at volume: {backgroundMusicVolume}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot play background music. Music source or clip is null.");
        }
    }

    public void StopAllSounds()
    {
        sfxSource.Stop();
        musicSource.Stop();
    }

    public void PlayTestSound()
    {
        if (testSoundEffect != null)
        {
            sfxSource.PlayOneShot(testSoundEffect, soundEffectsVolume);
        }
        else
        {
            Debug.LogWarning("Test sound effect is not assigned in SoundManager!");
        }
    }
}

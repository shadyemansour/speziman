using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private Dictionary<string, AudioClip> audioClips;
    private List<AudioClip> dieSounds;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources[0];
        sfxSource = sources[1];
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
    }

    public float PlaySound(string clipKey)
    {
        float audioLength = 0;
        if (audioClips.TryGetValue(clipKey, out AudioClip clip))
        {
            audioLength = clip.length;
            sfxSource.PlayOneShot(clip);
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
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}

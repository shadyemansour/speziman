using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauliActions : MonoBehaviour, IActions
{
    private RandomLightController lights;
    public System.Action[] Callbacks => _callbacks;
    private BarbaraController[] bars;
    [SerializeField] private GameObject hearts;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject brokenHeart;
    [SerializeField] private GameObject helmut;
    private PauliCopterController pauli;

    // [SerializeField] private PauliController pauli;


    public System.Action[] _callbacks = new System.Action[] { };

    public string[] Messages => _messages;

    private string[] _messages = new string[]
    {
        "Even though Helmut managed to make Spezi a well-known drink throughout Germany and beyond, his success was overshadowed by betrayal.",
        "Due to a lost bet, his friend Pauli stole his hard work and started using the name 'Spezi' for his own drink.",
        "Helmut was furious that his former friend had exploited him so shamelessly.",
        "Determined to put an end to Pauli's deceit, he decided to go to Munich to confront him.",
        "But little did he know, Pauli was already on his way to take everything from him."
    };


    void Awake()
    {
        lights = FindObjectOfType<RandomLightController>();
        bars = FindObjectsOfType<BarbaraController>();
        pauli = FindObjectOfType<PauliCopterController>();
    }

    public void NextLevel()
    {
        Debug.Log("Next Level");
        SoundManager.Instance.FadeOutSfxSound();
        GameManager.Instance.LoadNextLevel(true);
    }


    public void Actions(int count)
    {
        switch (count)
        {
            case 1:
                foreach (var bar in bars)
                {
                    bar.WaveNoPos();
                }
                lights.StartLights();
                hearts.SetActive(true);


                _callbacks[0]?.Invoke();
                break;
            case 2:
                foreach (var bar in bars)
                {
                    bar.StopWaveNoPos();
                }
                lights.StopLights();
                hearts.SetActive(false);
                brokenHeart.SetActive(true);
                _callbacks[0]?.Invoke();
                break;
            case 3:
                StartCoroutine(HelmutRed());
                foreach (var bar in bars)
                {
                    bar.MoveAway();
                }
                smoke.SetActive(true);
                _callbacks[0]?.Invoke();
                break;
            case 4:
                _callbacks[0]?.Invoke();
                break;
            case 5:
                pauli.MoveToTarget();
                _callbacks[0]?.Invoke();
                _callbacks[1]?.Invoke();
                break;
        }
    }

    private IEnumerator HelmutRed()
    {
        SpriteRenderer renderer = helmut.GetComponent<SpriteRenderer>();
        Color initialColor = renderer.color; // Capture the original color
        Color targetColor = new Color(1.0f, 0.4f, 0.4f, 1.0f);

        float timer = 0;
        float transitionDuration = 1f; // Set the duration of the color transition
        while (timer < transitionDuration)
        {
            // Lerp the color, which interpolates between the initial color and the target color based on 'timer/transitionDuration'
            renderer.color = Color.Lerp(initialColor, targetColor, timer / transitionDuration);
            timer += Time.deltaTime; // Increment the timer by the elapsed time since last frame
            yield return null; // Wait until the next frame, then continue execution from here (loop continues)
        }

        renderer.color = targetColor; // Ensure the final color is exactly the target color
    }


    public void SetCallbacks(System.Action[] callbacks)
    {
        _callbacks = callbacks;
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; } // Singleton instance

    [SerializeField] private float timeRemaining = 900; // Example start time in seconds (15 minutes)
    [SerializeField] private bool timerIsRunning = true;
    [SerializeField] private string timerDisplayName = "TimerText"; // Name of the GameObject with the TextMeshPro component
    private TextMeshProUGUI timerDisplay; // TextMeshPro component for displaying the timer
    public UnityEvent onTimerEnd; // Event triggered when timer ends


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the timer alive across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
    }

    void Start()
    {
        // Find and assign the TextMeshPro component from the specified GameObject
        GameObject displayObject = GameObject.Find(timerDisplayName);
        onTimerEnd.AddListener(GameManager.Instance.HandleTimerEnd);
        if (displayObject != null)
        {
            timerDisplay = displayObject.GetComponent<TextMeshProUGUI>();
        }

        if (timerIsRunning)
        {
            StartTimer(timeRemaining);
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                UpdateDisplay();
                onTimerEnd.Invoke();
                var gameOver = FindObjectOfType<GameOverScript>();
                gameOver.ShowButtons();
            }
        }
    }

    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    private void UpdateDisplay()
    {
        if (timerDisplay != null)
        {
            // Convert timeRemaining to minutes and seconds
            int minutes = (int)timeRemaining / 60;
            int seconds = (int)timeRemaining % 60;

            // Update the text component
            timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}

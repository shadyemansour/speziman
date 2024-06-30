using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private float initialTime = 900; // Example start time in seconds (15 minutes)
    [SerializeField] private float timeRemaining;
    [SerializeField] private bool timerIsRunning = true;
    [SerializeField] private string timerDisplayName = "TimerText"; // Name of the GameObject with the TextMeshPro component
    private TextMeshProUGUI timerDisplay; // TextMeshPro component for displaying the timer
    public UnityEvent onTimerEnd; // Event triggered when timer ends
    [SerializeField] private Animator animator; 

    void Awake()
    {
        // Find and assign the TextMeshPro component from the specified GameObject
        GameObject displayObject = GameObject.Find(timerDisplayName);
        if (displayObject != null)
        {
            timerDisplay = displayObject.GetComponent<TextMeshProUGUI>();
            animator = timerDisplay.GetComponent<Animator>();
        }

        // ResetTimer();
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
            }
        }
    }

    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        initialTime = duration;
        timerIsRunning = true;

        animator.Play("TimerText");
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    public void ResetTimer()
    {
        StartTimer(initialTime);
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

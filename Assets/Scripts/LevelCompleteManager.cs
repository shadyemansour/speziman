using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI itemsText;
    [SerializeField] private TextMeshProUGUI deliveriesText;
    [SerializeField] private Image[] barbaraHeads;


    [Header("Buttons")]
    [SerializeField] private GameObject levelCompleteButtons;
    [SerializeField] private GameObject gameOverButtons;

    [Header("Barbara Head Sprites")]
    [SerializeField] private Sprite yellowBarbaraHead;
    [SerializeField] private Sprite pinkBarbaraHead;
    [SerializeField] private Animator uiAnimator;
    [SerializeField] private Animator backgroundAnimator;

    [SerializeField] private Button nextLevelButton;


    private int currentLevel;


    public void ExitToMenu()
    {
        GameManager.Instance.LoadMenu();
    }

    public void RestartLevel()
    {
        GameManager.Instance.RestartLevel();

    }

    public void LoadNextLevel()
    {
        GameManager.Instance.LoadNextLevel();

    }
    public void UpdateUI(float completionTime, int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries, bool complete, int level)
    {
        currentLevel = level;
        int calculatedScore = CalculateScore(completionTime, collectedItems, totalItems, reachedDeliveries, totalDeliveries);

        scoreText.text = $"score \n{calculatedScore}";
        timeText.text = $"time \n{FormatTime(completionTime)}";

        // Handle level-specific UI elements
        switch (currentLevel)
        {
            case 1:
                UpdateLevelUI(collectedItems, totalItems, "Collected");
                break;
            case 2:
            case 3:
                UpdateLevelUI(collectedItems, totalItems, "Collected", reachedDeliveries, totalDeliveries);
                break;
            case 4:
                UpdateLevelUI(collectedItems, totalItems, "Destroyed", reachedDeliveries, totalDeliveries);
                break;
        }

        // Handle completion status
        if (complete)
        {
            titleText.text = "Level Complete!";
            levelCompleteButtons.SetActive(true);
            gameOverButtons.SetActive(false);
            UpdateButtonVisibility();
        }
        else
        {
            titleText.text = "Game Over :(";
            levelCompleteButtons.SetActive(false);
            gameOverButtons.SetActive(true);
        }

        backgroundAnimator.Play("BackgroundFadeIn");
        uiAnimator.Play("ZoomInAndFadeIn");

        // Return the calculated score to be used by GameManager
        GameManager.Instance.UpdatePlayerScore(calculatedScore);
    }
    private void SetBarbaraHeadsActive(bool active)
    {
        foreach (Image barbaraHead in barbaraHeads)
        {
            barbaraHead.gameObject.SetActive(active);
        }
    }

    private void UpdateBarbaraHeads(int reachedDeliveries, int totalDeliveries)
    {
        for (int i = 0; i < barbaraHeads.Length; i++)
        {
            barbaraHeads[i].sprite = (i < reachedDeliveries) ? yellowBarbaraHead : pinkBarbaraHead;
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    private void UpdateLevelUI(int collectedItems, int totalItems, string collectableText, int reachedDeliveries = -1, int totalDeliveries = -1)
    {
        bool isActive = true;
        if (currentLevel == 1)
        {
            scoreText.gameObject.transform.localPosition = new Vector3(0, 102.4f, 0);
            scoreText.gameObject.transform.localScale = new Vector3(2, 2, 2);
            itemsText.gameObject.transform.localPosition = new Vector3(0, -8.6f, 0);
            itemsText.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            timeText.gameObject.transform.localPosition = new Vector3(0, -60f, 0);
            timeText.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            isActive = false;

        }
        itemsText.gameObject.SetActive(true);
        itemsText.text = $"{collectableText} \n{collectedItems}/{totalItems}";
        deliveriesText.gameObject.SetActive(isActive);
        deliveriesText.text = $"deliveries \n{reachedDeliveries}/{totalDeliveries}";
        SetBarbaraHeadsActive(isActive);
    }

    private void UpdateLevel2And3UI(int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries, string collectableText)
    {
        itemsText.gameObject.SetActive(true);
        itemsText.text = $"{collectableText} items: {collectedItems}/{totalItems}";
        deliveriesText.gameObject.SetActive(true);
        SetBarbaraHeadsActive(true);
        UpdateBarbaraHeads(reachedDeliveries, totalDeliveries);
    }


    private void UpdateButtonVisibility()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(currentLevel < 4);
        }
        else
        {
            Debug.LogError("Next Level button reference is missing. Please assign it in the Inspector.");
        }
    }

    private int CalculateScore(float completionTime, int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries)
    {
        float timeScore = Mathf.Max(0, 1000 - (completionTime * 2)); // Decrease score as time increases
        float collectableScore = (float)collectedItems / totalItems * 1000; // Max 1000 points for collectables
        float deliveryScore = (float)reachedDeliveries / totalDeliveries * 1000; // Max 1000 points for deliveries

        int totalScore = Mathf.RoundToInt(timeScore + collectableScore + deliveryScore);
        return totalScore;
    }
}
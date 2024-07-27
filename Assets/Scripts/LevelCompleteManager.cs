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
    public void UpdateUI(float score, float completionTime, int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries, bool complete, int level)
    {
        currentLevel = level;

        scoreText.text = $"score \n{score}";
        timeText.text = $"time \n{FormatTime(completionTime)}";

        // Handle completion status
        if (complete)
        {
            switch (level)
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
            itemsText.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
            deliveriesText.gameObject.SetActive(false);
            scoreText.color = new Color(0.831f, 0f, 0.11f, 1f);
            scoreText.text = $"Time's up!";
            scoreText.fontSize = 30;
            scoreText.gameObject.transform.localPosition = new Vector3(0, 40f, 0);
            scoreText.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            SetBarbaraHeadsActive(false);
        }

        backgroundAnimator.Play("BackgroundFadeIn");
        uiAnimator.Play("ZoomInAndFadeIn");
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

}
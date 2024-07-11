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
    public void UpdateUI(int score, float completionTime, int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries, bool complete, int level)
    {
        currentLevel = level;
        scoreText.text = $"Score: {score}";
        timeText.text = $"Time: {FormatTime(completionTime)}";

        // Handle level-specific UI elements
        switch (currentLevel)
        {
            case 1:
                UpdateLevel1UI(collectedItems, totalItems);
                break;
            case 2:
            case 3:
                UpdateLevel2And3UI(collectedItems, totalItems, reachedDeliveries, totalDeliveries);
                break;
            case 4:
                UpdateLevel4UI(collectedItems, totalItems, reachedDeliveries, totalDeliveries);
                break;
        }

        if (complete)
        {
            titleText.text = "Level Complete!";
            levelCompleteButtons.SetActive(true);
            gameOverButtons.SetActive(false);
        }
        else
        {
            titleText.text = "Game Over :(";
            levelCompleteButtons.SetActive(false);
            gameOverButtons.SetActive(true);
        }

        // Handle Next Level button visibility
        Transform nextLevelButton = levelCompleteButtons.transform.Find("NextLevelButton");
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(currentLevel < 4);
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

    private void UpdateLevel1UI(int collectedItems, int totalItems)
    {
        itemsText.text = $"Collected items: {collectedItems}/{totalItems}";
        deliveriesText.gameObject.SetActive(false);
        SetBarbaraHeadsActive(false);
    }

    private void UpdateLevel2And3UI(int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries)
    {
        itemsText.text = $"Collected items: {collectedItems}/{totalItems}";
        deliveriesText.gameObject.SetActive(true);
        deliveriesText.text = $"Deliveries: {reachedDeliveries}/{totalDeliveries}";
        SetBarbaraHeadsActive(true);
        UpdateBarbaraHeads(reachedDeliveries, totalDeliveries);
    }

    private void UpdateLevel4UI(int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries)
    {
        itemsText.text = $"Destroyed items: {collectedItems}/{totalItems}";
        deliveriesText.gameObject.SetActive(true);
        deliveriesText.text = $"Deliveries: {reachedDeliveries}/{totalDeliveries}";
        SetBarbaraHeadsActive(true);
        UpdateBarbaraHeads(reachedDeliveries, totalDeliveries);
    }
}
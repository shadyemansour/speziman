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
    public void UpdateUI(int score, float completionTime, int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries, bool complete)
    {
        scoreText.text = $"Score: {score}";
        timeText.text = $"Time: {FormatTime(completionTime)}";
        itemsText.text = $"Collected items: {collectedItems}/{totalItems}";
        deliveriesText.text = $"Deliveries: {reachedDeliveries}/{totalDeliveries}";
        if (complete)
        {

            titleText.text = $"Level Complete!";
            levelCompleteButtons.gameObject.SetActive(true);
            gameOverButtons.gameObject.SetActive(false);
        }
        else
        {
            titleText.text = $"Game Over :(";
            levelCompleteButtons.gameObject.SetActive(false);
            gameOverButtons.gameObject.SetActive(true);
        }

        UpdateBarbaraHeads(reachedDeliveries, totalDeliveries);
        backgroundAnimator.Play("BackgroundFadeIn");
        uiAnimator.Play("ZoomInAndFadeIn");
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

   
}
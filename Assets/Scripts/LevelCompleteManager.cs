using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI itemsText;
    [SerializeField] private TextMeshProUGUI deliveriesText;
    [SerializeField] private Image[] barbaraHeads;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button newLevelButton;

    [Header("Barbara Head Sprites")]
    [SerializeField] private Sprite yellowBarbaraHead;
    [SerializeField] private Sprite pinkBarbaraHead;


    public void UpdateUI(int score, float completionTime, int collectedItems, int totalItems, int reachedDeliveries, int totalDeliveries)
    {
        scoreText.text = $"Score: {score}";
        timeText.text = $"Time: {FormatTime(completionTime)}";
        itemsText.text = $"Collected items: {collectedItems}/{totalItems}";
        deliveriesText.text = $"Deliveries: {reachedDeliveries}/{totalDeliveries}";

        UpdateBarbaraHeads(reachedDeliveries, totalDeliveries);
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

    public void GoToMenu()
    {
        Debug.Log("GoToMenu clicked");
        SceneManager.LoadScene("StartScene");
    }

    public void RestartLevel()
    {
        Debug.Log("RestartLevel clicked");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        Debug.Log("LoadNextLevel clicked");
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex + 1);
    }
}
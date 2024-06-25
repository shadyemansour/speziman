using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteScript : MonoBehaviour
{
    public GameObject levelCompleteCanvas;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI collectedItemsText;

    void Start()
    {
        levelCompleteCanvas.SetActive(false);
    }

    public void ShowLevelComplete(float finalTime, int collectedItems, int totalItems)
    {
        levelCompleteCanvas.SetActive(true);
        timeText.text = "Time: " + FormatTime(finalTime);
        collectedItemsText.text = "Collected Items: " + collectedItems + "/" + totalItems;
        Time.timeScale = 0f; // Pause the game
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene("MainMenu");
    }
}

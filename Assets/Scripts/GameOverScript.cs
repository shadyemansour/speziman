using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public static GameOverScript Instance { get; private set; }

    private Button[] buttons;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get the buttons
        buttons = GetComponentsInChildren<Button>(true); // Include inactive buttons
    }

    void OnEnable()
    {
        // Show buttons when the GameObject is enabled
        ShowButtons();
    }

    public void HideButtons()
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    public void ShowButtons()
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(true);
            Debug.Log($"Button '{b.name}' shown");
        }
    }

    public void ExitToMenu()
    {
        // Reload the level
        Application.LoadLevel("StartScene");
    }

    public void RestartGame()
    {
        // Reload the level; Needs to be made adaptable
        GameManager.Instance.LoadLevel(1);
        // int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;  -> more adaptable
        // GameManager.Instance.LoadLevel(currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        GameManager.Instance.LoadLevel(2);
        // int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;   -> more adaptable
        // GameManager.Instance.LoadLevel(currentLevelIndex + 1);
    }
}
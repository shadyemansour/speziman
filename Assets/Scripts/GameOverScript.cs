using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public static GameOverScript Instance { get; private set; }

    private Button[] buttons;
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
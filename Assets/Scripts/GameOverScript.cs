using System.Collections; // Not used in this script but commonly included for coroutines
using UnityEngine; // Required for Unity engine functionality
using UnityEngine.UI; // Required for UI elements
using UnityEngine.SceneManagement; // Required for scene management

/// <summary>
/// Start or quit the game
/// </summary>
public class GameOverScript : MonoBehaviour
{
    // Singleton instance
    public static GameOverScript Instance { get; private set; }

    // Array to hold references to the UI buttons
    private Button[] buttons;

    // Called when the script instance is being loaded
    void Awake()
    {
        // Ensure only one instance of GameOverScript exists (singleton pattern)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get all Button components in the children of this GameObject
        buttons = GetComponentsInChildren<Button>();

        // Disable all buttons initially
        HideButtons();
    }

    // Method to hide all buttons
    public void HideButtons()
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    // Method to show all buttons
    public void ShowButtons()
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(true);
            Debug.Log("Buttons Shown"); // Log to console for debugging purposes
        }
    }

    // Method to exit to the main menu
    public void ExitToMenu()
    {
        // Load the StartScene
        SceneManager.LoadScene("StartScene");
    }

    // Method to restart the game
    public void RestartGame()
    {
        // Reload the level using GameManager's LoadLevel method
        GameManager.Instance.LoadLevel(1);
    }
}

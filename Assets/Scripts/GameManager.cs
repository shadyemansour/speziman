using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Stats
    public int totalCola;
    public int totalOranges;
    public int collectedCola = 0;
    public int collectedOranges = 0;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method to load a new level
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        ResetCollectables();
    }

    // Update the collectable counts when loading a new level
    private void ResetCollectables()
    {
        // Assume these values are determined by each level
        totalCola = FindObjectsOfType<Collectable>().Length;  // Update to reflect actual items in the scene
        totalOranges = FindObjectsOfType<Collectable>().Length;  // Update to reflect actual items in the scene
        collectedCola = 0;
        collectedOranges = 0;

        UpdateUI();
    }

    // Call this method to update the UI
    public void UpdateUI()
    {
        // Update UI elements here
        Debug.Log("Update UI with new values.");
    }

    public void CollectItem(string itemType)
    {
        if (itemType == "Cola")
        {
            collectedCola++;
            Debug.Log("Cola collected! "+ collectedCola);
        }
        else if (itemType == "Orange")
        {
            collectedOranges++;
            Debug.Log("Orange collected! "+ collectedOranges);
        }
        UpdateUI();
    }
}

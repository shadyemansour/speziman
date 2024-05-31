using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class  GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Stats
    public int totalCola;
    public int totalOranges;
    public int collectedCola = 0;
    public int collectedOranges = 0;
    public Slider colaSlider;
    public Slider orangeSlider;

     public GameObject playerPrefab; 
    public Transform spawnPoint;  

    private GameObject currentPlayer; 

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

        SpawnPlayer();
    }

    void Start()
    {
        InitializeCollectables();

    }

    private void InitializeCollectables()
    {
        // Finding the "Collectables" GameObject and counting children
        GameObject collectables = GameObject.Find("Collectables");
        if (collectables != null)
        {
            totalCola = 0;
            totalOranges = 0;

            foreach (Transform item in collectables.transform)
            {
                if (item.name.Contains("Cola"))
                    totalCola++;
                else if (item.name.Contains("Orange"))
                    totalOranges++;
            }

            // Set sliders
            if (colaSlider != null && orangeSlider != null)
            {
                colaSlider.maxValue = totalCola;
                orangeSlider.maxValue = totalOranges;
                colaSlider.value = collectedCola;
                orangeSlider.value = collectedOranges;
            }
        }

        ResetCollectables();
    }

public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (currentPlayer != null) Destroy(currentPlayer); // Destroy existing player instance if any

        if (playerPrefab != null && spawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player prefab or spawn point not set.");
        }
    }
    // Update the collectable counts when loading a new level
   private void ResetCollectables()
    {
        // Reset and update stats
        collectedCola = 0;
        collectedOranges = 0;

        // Reset sliders
        if (colaSlider != null && orangeSlider != null)
        {
            colaSlider.maxValue = totalCola;
            orangeSlider.maxValue = totalOranges;
            colaSlider.value = collectedCola;
            orangeSlider.value = collectedOranges;
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        // Update UI sliders
        if (colaSlider != null && orangeSlider != null)
        {
            colaSlider.value = collectedCola;
            orangeSlider.value = collectedOranges;
        }
    }

    public void CollectItem(string itemType)
    {
        if (itemType == "Cola")
        {
            collectedCola++;
        }
        else if (itemType == "Orange")
        {
            collectedOranges++;
        }
        UpdateUI();
    }
}


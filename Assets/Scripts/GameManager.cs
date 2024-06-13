using System;
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
    private Vector3 lastCheckpointPosition;
    Cinemachine.CinemachineVirtualCamera vcam;

    [SerializeField] private float[] levelDurations = {300f, 450f, 600f}; // Durations in seconds for each level


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


        // SpawnPlayer();
    }

    void Start()
    {
        InitializeCollectables();
        if (spawnPoint != null) lastCheckpointPosition = spawnPoint.position;
        vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (currentPlayer == null) currentPlayer = GameObject.FindWithTag("Player");
        vcam.Follow = currentPlayer.transform;
       Debug.Log("vcam: " + vcam);


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

    public void LoadLevel(int sceneNumber)
    {
        SceneManager.LoadScene("Level"+sceneNumber.ToString());
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event 

    }

        public void LoadIntro()
    {
        SceneManager.LoadScene("Intro");
    }

    

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Attempt to load the spawn point for the loaded scene
        LoadSpawnPoint(scene.name + "SpawnPoint");
        SpawnPlayer();
        int sceneNumber = int.Parse(scene.name.Replace("Level", ""));
        Timer.Instance.StartTimer(levelDurations[sceneNumber - 1]);

        // Unsubscribe to prevent this from being called if another scene is loaded elsewhere
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadSpawnPoint(string spawnPointName)
    {
        Debug.Log("Spawn point name: " + spawnPointName);

        GameObject spawnPointPrefab = Resources.Load<GameObject>("SpawnPoints/" + spawnPointName);
        if (spawnPointPrefab != null)
        {
            this.spawnPoint = Instantiate(spawnPointPrefab).transform;
        }
        else
        {
            Debug.LogError("Spawn point prefab not found for: " + spawnPointName);
        }
    }


    void SpawnPlayer()
    {
        if (currentPlayer != null) Destroy(currentPlayer); // Destroy existing player instance if any

        if (playerPrefab != null && spawnPoint != null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            
            lastCheckpointPosition = spawnPoint.position;
        }
        else
        {
            Debug.LogError("Player prefab or spawn point not set.");
        }
    }
    public void UpdateLastCheckpoint(Vector3 newCheckpointPosition)
    {
        // Check if the new checkpoint is further to the right than the last saved one
        if (newCheckpointPosition.x > lastCheckpointPosition.x)
        {
            lastCheckpointPosition = newCheckpointPosition;
            Debug.Log("Updated checkpoint position: " + lastCheckpointPosition);
        }
    }


    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = lastCheckpointPosition;
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

    public void HandleTimerEnd()
    {
        Debug.Log("Time's up! Game over or level fail.");
        SoundManager.Instance.FadeOutBackgroundSound();
        SoundManager.Instance.PlaySound("levelFailed");
        
    }
}


using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class  GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Stats
    [SerializeField] private int totalCola;
    [SerializeField] private  int totalOranges;
    // [SerializeField] private  Slider colaSlider;
    // [SerializeField] private  Slider orangeSlider;

     [SerializeField] private  GameObject playerPrefab; 
    [SerializeField] private  Transform spawnPoint;  

    private GameObject currentPlayer; 
    private Vector3 lastCheckpointPosition;
    CinemachineVirtualCamera vcam;



    public int score = 0;
    public int totalDeliveries = 3;
    public int reachedDeliveries = 0;
    private float levelStartTime;       
    [SerializeField] private GameObject levelCompleteScreen;


    private Timer timerInstance;

    [SerializeField] private GameObject timerPrefab; 


    [SerializeField] private float[] levelDurations = {90f, 450f, 600f}; // Durations in seconds for each level
    private Dictionary<string, List<Collectable>> collectables = new Dictionary<string, List<Collectable>>();
    private Dictionary<string, TextMeshProUGUI> collectableTexts = new Dictionary<string, TextMeshProUGUI>();

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
        // InitializeCollectables();
        if (spawnPoint != null) lastCheckpointPosition = spawnPoint.position;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    //     vcam = FindObjectOfType<CinemachineVirtualCamera>();
    //     Debug.Log("vcam: " + vcam);
    //     if (currentPlayer == null) currentPlayer = GameObject.FindWithTag("Player");

    //     vcam.Follow = currentPlayer.transform;
    //    Debug.Log("vcam: " + vcam);

        levelStartTime = Time.time;


    }

    void FindTextObjects()
    {
        foreach (var pair in collectables)
        {
            TextMeshProUGUI text = GameObject.FindGameObjectWithTag(pair.Key + "_Text").GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                collectableTexts[pair.Key] = text;
            }
            else
            {
                Debug.LogError("Text object not found for: " + pair.Key);
            }
        }
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

    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)         // TODO: fix level restarts (levelcompletescreen not triggered, no dies, ...)
    {
        InitializeCollectables();
        InstantiateTimer();
        InitializeUI();


        // Attempt to load the spawn point for the loaded scene
        LoadSpawnPoint(scene.name + "SpawnPoint");
        SpawnPlayer();
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        var confiner = FindObjectOfType<CinemachineConfiner2D>();
        var gridCollider = GameObject.FindWithTag("Ground")?.GetComponent<PolygonCollider2D>();
        var currentPlayer = GameObject.FindWithTag("Player");
        vcam.Follow = currentPlayer.transform;
        confiner.m_BoundingShape2D = gridCollider;
        int sceneNumber = int.Parse(scene.name.Replace("Level", ""));
        timerInstance.StartTimer(levelDurations[sceneNumber - 1]);
        // Unsubscribe to prevent this from being called if another scene is loaded elsewhere
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadSpawnPoint(string spawnPointName)
    {
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
   private void InitializeCollectables()
    {
        Collectable[] allCollectables = FindObjectsOfType<Collectable>();
        foreach (Collectable collectable in allCollectables)
        {
            if (!collectables.ContainsKey(collectable.itemType))
            {
                collectables[collectable.itemType] = new List<Collectable>();
            }
            collectables[collectable.itemType].Add(collectable);
        }
    }

    public void CollectItem(Collectable collectable)
    {
        if (!collectable.collectedAfterCheckpoint)
        {
            collectable.collectedAfterCheckpoint = true;
            UpdateUI(collectable.itemType, 1);
            score += collectable.scoreValue; 
        }
    }

    public void ResetCollectables()
    {
        foreach (var pair in collectables)
        {
            foreach (Collectable collectable in pair.Value)
            {
                if (collectable.collectedAfterCheckpoint)
                {
                    collectable.ResetCollectable();
                }
            }
        }
        ResetUI();  
    }

    public void UpdateLastCheckpoint(Vector3 newCheckpointPosition)
    {
        if (newCheckpointPosition.x > lastCheckpointPosition.x)
        {
            lastCheckpointPosition = newCheckpointPosition;
        }
        foreach (var pair in collectables)
        {
            foreach (Collectable collectable in pair.Value)
            {
                collectable.collectedAfterCheckpoint = false;
            }
        }
    }

    private void InitializeUI()
    {
        FindTextObjects();
        ResetCollectablesTexts();
    }

    private void ResetCollectablesTexts(){
        foreach(var pair in collectableTexts)
        {
            pair.Value.text = "0";
        }
    }

    private void ResetUI()
    {
        ResetCollectablesTexts();

        foreach (var list in collectables.Values)
        {
            foreach (Collectable collectable in list)
            {
                if (!collectable.gameObject.activeSelf)
                {
                    UpdateUI(collectable.itemType, 1);
                }
            }
        }
    }

     private void UpdateUI(string itemType, int change)
    {
        if (collectableTexts.ContainsKey(itemType))
        {
            collectableTexts[itemType].text = (int.Parse(collectableTexts[itemType].text) + change).ToString();
        }
        else{
            Debug.LogError("Text object not found for: " + itemType);
        }
    }

     public void RespawnPlayer(GameObject player)
    {
        ResetCollectables();
        player.GetComponent<PlayerMovement>().ResetSpeed();
        player.transform.position = lastCheckpointPosition;
    }

    public void HandleTimerEnd()
    {
        Debug.Log("Time's up! Game over or level fail.");
        SoundManager.Instance.FadeOutBackgroundSound();
        SoundManager.Instance.PlaySound("levelFailed");
        
    }

   private void InstantiateTimer()
    {
        if (timerInstance != null)
        {
            Destroy(timerInstance.gameObject);
        }

        GameObject timerObject = Instantiate(timerPrefab);
        timerInstance = timerObject.GetComponent<Timer>();

        if (timerInstance != null)
        {
            timerInstance.onTimerEnd.AddListener(HandleTimerEnd);
        }
    }
  
    private float CalculateLevelCompletionTime()
    {
        float currentTime = Time.time;
        return currentTime - levelStartTime;
    }


    public void UpdateDeliveries()
    {
        reachedDeliveries = Mathf.Min(reachedDeliveries + 1, totalDeliveries);
    }

    public void TriggerLevelComplete()
    {
        Debug.Log("TriggerLevelComplete called");
        ActivateEndCanvas(true);
    }
    public void TriggerGameOver()
    {
        Debug.Log("TriggerLevelComplete called");
        ActivateEndCanvas(false);
    }

    public void ActivateEndCanvas(bool complete)
    {
        float completionTime = CalculateLevelCompletionTime();
        int collectedItems = GetCollectedItemsCount();
        int totalItems = GetTotalItemsCount(); 

        if (complete) {
        if (levelCompleteScreen != null)
            {
                levelCompleteScreen.SetActive(true);
                Debug.Log("Level complete screen activated");

                LevelCompleteManager lcManager = levelCompleteScreen.GetComponent<LevelCompleteManager>();
                if (lcManager != null)
                {
                    lcManager.UpdateUI(score, completionTime, collectedItems, totalItems, reachedDeliveries, totalDeliveries);
                    Debug.Log("LevelCompleteManager UpdateUI called");
                }
                else
                {
                    Debug.LogError("LevelCompleteManager component not found on levelCompleteScreen");
                }
            }
            else
            {
                Debug.LogError("levelCompleteScreen is null in GameManager");
            }
        }
    }

private int GetCollectedItemsCount()
{
    int count = 0;
    foreach (var pair in collectables)
    {
        foreach (var collectable in pair.Value)
        {
            if (!collectable.gameObject.activeSelf)
            {
                count++;
            }
        }
    }
    return count;
}

    private int GetTotalItemsCount()
    {
        int count = 0;
        foreach (var pair in collectables)
        {
            count += pair.Value.Count;
        }
        return count;
    }



    // todo call method
    public void IncrementDeliveries()
    {
        reachedDeliveries = Mathf.Min(reachedDeliveries + 1, totalDeliveries);
    }
    
}


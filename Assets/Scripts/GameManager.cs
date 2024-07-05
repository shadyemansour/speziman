using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using System.Text;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;
using Unity.Collections;

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
    private GameObject levelCompleteScreen;
    
    public int currentLevel =1;


    private Timer timerInstance;

    [SerializeField] private GameObject timerPrefab; 


    [SerializeField] private float[] levelDurations = {90f, 450f, 600f}; // Durations in seconds for each level
    private Dictionary<string, List<Collectable>> collectables = new Dictionary<string, List<Collectable>>();
    private Dictionary<string, TextMeshProUGUI> collectableTexts = new Dictionary<string, TextMeshProUGUI>();
    [SerializeField] private Dictionary<string, GameObject> collectablePrefabs = new Dictionary<string, GameObject>();

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
    
    public void LoadNextLevel(bool wasCutScene=false)
    {
        if (currentLevel < 3)
        {
            if (wasCutScene)
            {
                LoadLevel(currentLevel, false);
            }
            LoadLevel(++currentLevel, false);
        }
        else
        {
            LoadIntro();
        }
    }
    public void RestartLevel()
    {
        this.LoadLevel(currentLevel, false);
    }


    public void LoadLevel(int sceneNumber, bool isCut)
    {
        currentLevel = sceneNumber;
        string levelName = isCut ? "Cut" : "Level";
        levelName += sceneNumber.ToString();
        SceneManager.LoadScene(levelName);
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event 

    }

    public void LoadIntro()
    {
        SceneManager.LoadScene("Intro");
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)         // TODO: fix level restarts (levelcompletescreen not triggered, no dies, ...)
    {
        levelCompleteScreen = GameObject.FindGameObjectWithTag("LevelCompleteUI");
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name.ToLower().Contains("level"))
        {   
            InitializeCollectables();
            // InitializeCollectablesUI();
            InstantiateTimer();
            InitializeUI();
            // Attempt to load the spawn point for the loaded scene
            LoadSpawnPoint(scene.name + "SpawnPoint");
            InitializeBackground();
            SpawnPlayer();
            SetupCamera();
        
            int sceneNumber = int.Parse(scene.name.Replace("Level", ""));
            currentLevel = sceneNumber;
            timerInstance.StartTimer(levelDurations[sceneNumber - 1]);
        }
        // Unsubscribe to prevent this from being called if another scene is loaded elsewhere
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SetupCamera()
    {
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();
        var confiner = FindObjectOfType<CinemachineConfiner2D>();
        var gridCollider = GameObject.FindWithTag("Grid")?.GetComponent<PolygonCollider2D>();
        var currentPlayer = GameObject.FindWithTag("Player");
        vcam.Follow = currentPlayer.transform;
        confiner.m_BoundingShape2D = gridCollider;
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
        collectables.Clear();
        foreach (Collectable collectable in allCollectables)
        {
            if (!collectables.ContainsKey(collectable.itemType))
            {
                collectables[collectable.itemType] = new List<Collectable>();
            }
            collectables[collectable.itemType].Add(collectable);
        }
        LoadCollectablesPrefabs();
    }

    //  private void InitializeCollectablesUI()
    // {
    //     GameObject parent = GameObject.FindGameObjectWithTag("StatsCanvas");
    //     Vector3 nextPosition = new Vector3(-344, 186, 0); // Starting position for the first item
    //     float xSpacing = 32.0f; // Horizontal spacing between UI and text
    //     float ySpacing = 6.0f; // Vertical spacing between lines

    //     foreach (var pair in collectables)
    //     {
    //         Debug.Log($"{pair.Key}:" );
    //         Debug.Log("currentPosition: " + nextPosition);
    //         // Load and instantiate the UI prefab
    //         Transform uiPrefab = Resources.Load<Transform>($"UI/{pair.Key}");
    //         if (uiPrefab == null) {
    //             Debug.LogError($"Prefab for {pair.Key} not found in Resources/UI/");
    //             continue;
    //         }
    //         Transform uiInstance = Instantiate(uiPrefab, parent.transform);
    //         uiInstance.localPosition = nextPosition;

    //         // Assuming the prefab has a RectTransform component
    //         RectTransform uiRect = uiInstance.GetComponent<RectTransform>();
    //         if (uiRect == null) {
    //             Debug.LogError("No RectTransform found on UI prefab: " + pair.Key);
    //             continue;
    //         }
    //         Debug.Log("uiInstance position: " + uiInstance.transform.localPosition);
    //         // uiRect.anchoredPosition = nextPosition;

    //         // Calculate the next position for the text, aligning it next to the UI
    //         Vector3 textPosition = new Vector3(nextPosition.x + xSpacing, nextPosition.y, 0);
    //         Debug.Log("text position: " +textPosition);

    //         // Load and instantiate the text prefab
    //         Transform textPrefab = Resources.Load<Transform>($"UI/{pair.Key}_Text");
    //         if (textPrefab == null) {
    //             Debug.LogError($"Text prefab for {pair.Key} not found in Resources/UI/");
    //             continue;
    //         }
    //         Transform textInstance = Instantiate(textPrefab, parent.transform);
    //         // RectTransform textRect = textInstance.GetComponent<RectTransform>();
    //         textInstance.localPosition = new Vector2(nextPosition.x + xSpacing, nextPosition.y);

    //         Debug.Log("textInstance position: " + textInstance.transform.localPosition);
    //         // Move the next starting position down to the next line after both UI and text are placed
    //         nextPosition.y -= (uiRect.rect.height/2 + ySpacing);
    //     }
    // }


    private void LoadCollectablesPrefabs()
    {
        collectablePrefabs.Clear();
        foreach (var pair in collectables)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Prefabs/");
            sb.Append(pair.Key.ToLower());
            sb.Append("Anim");
            GameObject prefab = Resources.Load<GameObject>(sb.ToString());
            if (prefab != null)
            {
                collectablePrefabs[pair.Key] = prefab;
            }
            else
            {
                Debug.LogError("Prefab not found for: " + pair.Key);
            }
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

    private void InitializeBackground()
    {
        
        GameObject[] background = GameObject.FindGameObjectsWithTag("Background");
        if (background != null)
        {
            foreach (GameObject bg in background)
            {
                ParallaxBackground parallaxBackground = bg.GetComponent<ParallaxBackground>();
                if (parallaxBackground != null)
                {
                    parallaxBackground.SetLevelPlayerSpawnPoint(spawnPoint);
                    parallaxBackground.InitializeBackground();
                }
                else
                {
                    Debug.LogError("ParallaxBackground component not found on background object");
                }
            }
        }
        else
        {
            Debug.LogError("Background object not found");
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
            pair.Value.text = "0/"+collectables[pair.Key].Count.ToString();
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
            StringBuilder sb = new StringBuilder();
            sb.Append((int.Parse(collectableTexts[itemType].text.Split("/")[0]) + change).ToString());
            sb.Append("/");
            sb.Append(collectables[itemType].Count.ToString());
            collectableTexts[itemType].text = sb.ToString();
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
        TriggerGameOver();
        
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

    private void StopLevel()
    {
        
        currentPlayer.GetComponent<PlayerMovement>().SetIsStopped(true);
        timerInstance.StopTimer();

    }

    public void ActivateEndCanvas(bool complete)
    {
        StopLevel();
        float completionTime = CalculateLevelCompletionTime();
        int collectedItems = GetCollectedItemsCount();
        int totalItems = GetTotalItemsCount(); 

            if (levelCompleteScreen != null)
                {
                    levelCompleteScreen.GetComponent<CanvasGroup>().alpha = 1;
                    Debug.Log("Level complete screen activated");

                    LevelCompleteManager lcManager = levelCompleteScreen.GetComponent<LevelCompleteManager>();
                    if (lcManager != null)
                    {
                        Debug.Log(reachedDeliveries);
                        lcManager.UpdateUI(score, completionTime, collectedItems, totalItems, reachedDeliveries, totalDeliveries, complete);
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

    public void SendCollectables(GameObject player, Vector3 barbaraPosition )
    {
            StartCoroutine(SendCollectablesCoroutine(player, barbaraPosition, 0.2f));
    }

    private IEnumerator SendCollectablesCoroutine(GameObject player, Vector3 barbaraPosition, float delayBetweenItems)
    {   
        player.GetComponent<PlayerMovement>().SetIsStopped(true);

        foreach (var pair in collectableTexts)
        {

            int value = int.Parse(collectableTexts[pair.Key].text.Split("/")[0]);
            if(value > 0 ){
                for (int i = 0; i < value; i++)
                {
                    GameObject instance = Instantiate(collectablePrefabs[pair.Key], player.transform.position, Quaternion.identity);
                    StartCoroutine(MoveCollectableInParabola(instance, barbaraPosition, 1.0f));
                    yield return new WaitForSeconds(delayBetweenItems); 

                    
                }
            }
            player.GetComponent<PlayerMovement>().SetIsStopped(false);
        }
    }

    IEnumerator MoveCollectableInParabola(GameObject collectable, Vector3 targetPos, float duration)
{
    float time = 0;
    Vector3 startPos = collectable.transform.position;
    float height = Mathf.Abs(targetPos.y - startPos.y) / 2 + 2; // Height of the parabola; adjust as needed

    while (time < duration)
    {
        float t = time / duration; // Normalize time
        // Interpolate the x and y positions
        collectable.transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, height * Mathf.Sin(Mathf.PI * t), 0);
        time += Time.deltaTime;
        yield return null;
    }

    collectable.transform.position = targetPos; // Ensure it ends exactly at the target
    Destroy(collectable); // Destroy the object after reaching the target
}


    
}


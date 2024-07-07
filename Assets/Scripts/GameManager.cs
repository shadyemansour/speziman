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
using UnityEditor.ShaderGraph.Internal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private int totalCola;
    [SerializeField] private int totalOranges;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float[] levelDurations = { 90f, 450f, 600f, 700f }; // Durations in seconds for each level
    [SerializeField] private GameObject timerPrefab;

    private GameObject currentPlayer;
    private GameObject levelCompleteScreen;
    private Vector3 lastCheckpointPosition;
    private Timer timerInstance;

    public int score = 0;
    private int totalDeliveries = 3;
    public int reachedDeliveries = 0;
    private float levelStartTime;
    public int currentLevel = 1;
    private int deliverablesCount = 3;
    private int maxLevels = 3;


    //Collectables
    private Dictionary<string, List<Collectable>> collectables = new Dictionary<string, List<Collectable>>();
    private Dictionary<string, TextMeshProUGUI> collectableTexts = new Dictionary<string, TextMeshProUGUI>();
    private Dictionary<string, GameObject> collectablePrefabs = new Dictionary<string, GameObject>();

    //GameStats
    private static string dataPath;
    public GameData gameData;
    private PlayerData currentPlayerData;
    public bool ShowLevelSelectOnLoad { get; set; }



    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(gameObject);
            dataPath = Application.persistentDataPath + "/gameData.json";
            Debug.Log("Data path: " + dataPath);
            gameData = new GameData();
            ShowLevelSelectOnLoad = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadGameData();

        if (spawnPoint != null) lastCheckpointPosition = spawnPoint.position;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        levelStartTime = Time.time;
    }

    void FindTextObjects()
    {
        collectableTexts.Clear();
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

    public void LoadNextLevel(bool wasCutScene = false)
    {
        if (currentLevel >= maxLevels)
        {
            LoadMenu();
        }
        else if (wasCutScene)
        {
            LoadLevel(currentLevel++, !wasCutScene);
        }
        else if (currentLevel < maxLevels - 1)
        {
            LoadLevel(++currentLevel, !wasCutScene);
        }
        else
        {
            LoadMenu();
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
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    public void LoadMenu()
    {
        if (currentPlayerData != null)
        {
            ShowLevelSelectOnLoad = true;
        }
        SceneManager.LoadScene("StartScene");
    }





    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelCompleteScreen = GameObject.FindGameObjectWithTag("LevelCompleteUI");
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name.ToLower().Contains("level"))
        {
            InitializeCollectables();
            // InitializeCollectablesUI();
            InstantiateTimer();
            InitializeUI();
            LoadSpawnPoint(scene.name + "SpawnPoint");
            InitializeBackground();
            SpawnPlayer();
            SetupCamera();


            int sceneNumber = int.Parse(scene.name.Replace("Level", ""));
            Debug.Log("Scene number: " + sceneNumber);
            currentLevel = sceneNumber;
            timerInstance.StartTimer(levelDurations[sceneNumber - 1]);
        }
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
        if (currentPlayer != null) Destroy(currentPlayer);

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

    private void ResetCollectablesTexts()
    {
        foreach (var pair in collectableTexts)
        {
            pair.Value.text = "0/" + collectables[pair.Key].Count.ToString();
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
        else
        {
            Debug.LogError("Text object not found for: " + itemType);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        DestroyShots();
        ResetCollectables();
        player.GetComponent<PlayerMovement>().ResetSpeed();

        player.transform.position = lastCheckpointPosition;
    }

    private void DestroyShots()
    {
        Shot[] shots = GameObject.FindObjectsOfType<Shot>();
        foreach (Shot shot in shots)
        {
            Destroy(shot.gameObject);
        }
    }

    public void HandleTimerEnd()
    {
        Debug.Log("Time's up! Game over or level fail.");
        SoundManager.Instance.FadeOutBackgroundSound();
        SoundManager.Instance.PlaySound("levelFailed");
        TriggerGameOver();

    }

    public void ToggleTimer()
    {

        currentPlayer.GetComponent<PlayerMovement>().SetIsStopped(timerInstance.TogglePause());
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
        UnlockLevel(currentLevel + 1);
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

    public void IncrementDeliveries()
    {

        Debug.Log("IncrementDeliveries called " + reachedDeliveries + "/" + totalDeliveries);
        reachedDeliveries = Mathf.Min(reachedDeliveries + 1, totalDeliveries);
        Debug.Log("reachedDeliveries: " + reachedDeliveries);
    }

    public bool SendCollectables(GameObject player, Vector3 barbaraPosition)
    {
        int value = int.Parse(collectableTexts.First().Value.text.Split("/")[0]);
        if (value > 0)
        {
            StartCoroutine(SendCollectablesCoroutine(player, barbaraPosition, 0.2f));
            return true;
        }
        else
        {
            return false;
        }

    }

    private IEnumerator SendCollectablesCoroutine(GameObject player, Vector3 barbaraPosition, float delayBetweenItems)
    {
        player.GetComponent<PlayerMovement>().SetIsStopped(true);

        foreach (var pair in collectableTexts)
        {

            int value = int.Parse(collectableTexts[pair.Key].text.Split("/")[0]);
            value = Mathf.Min(value, deliverablesCount);
            if (value > 0)
            {
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
        float height = Mathf.Abs(targetPos.y - startPos.y) / 2 + 2;

        while (time < duration)
        {
            float t = time / duration; // Normalize time
            // Interpolate the x and y positions
            collectable.transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, height * Mathf.Sin(Mathf.PI * t), 0);
            time += Time.deltaTime;
            yield return null;
        }

        collectable.transform.position = targetPos;
        Destroy(collectable);
    }

    /////    GameData    /////
    public bool RegisterNewPlayer(string playerName)
    {
        // Check if the username already exists
        if (gameData.players.Any(p => p.playerName == playerName))
        {
            Debug.Log("Username already exists. Choose a different username.");
            return false;
        }
        else
        {
            // Create new player data
            PlayerData newPlayer = new PlayerData()
            {
                playerName = playerName,
                score = 0,
                currentLevel = 1,
                unlockedLevels = new List<int>() { 1 }
            };

            gameData.players.Add(newPlayer);
            SaveGameData();
            currentPlayerData = newPlayer;

            Debug.Log("New player registered: " + playerName);


            return true;
        }
    }

    public bool LoginExistingPlayer(string playerName)
    {
        // Check if the username exists
        if (gameData.players.Any(p => p.playerName == playerName))
        {
            Debug.Log("Player found: " + playerName);
            currentPlayerData = gameData.players.Find(p => p.playerName == playerName);
            currentLevel = currentPlayerData.currentLevel;
            return true;
        }
        else
        {
            Debug.Log("Player not found: " + playerName);
            return false;
        }
    }

    public int GetPlayerMaxLevel()
    {
        if (currentPlayerData != null)
        {
            return currentPlayerData.unlockedLevels.Max();
        }
        else
        {
            Debug.LogWarning("No player data found.");
            return 1;
        }
    }

    public void SaveGameData()
    {
        if (currentPlayerData != null)
        {
            PlayerData existingPlayer = gameData.players.Find(p => p.playerName == currentPlayerData.playerName);
            if (existingPlayer != null)
            {
                int index = gameData.players.IndexOf(existingPlayer);
                gameData.players[index] = currentPlayerData;
            }
            else
            {
                gameData.players.Add(currentPlayerData);
            }
        }
        string json = JsonUtility.ToJson(gameData, true);
        System.IO.File.WriteAllText(dataPath, json);
        Debug.Log("Game data saved to " + dataPath);
    }

    public void LoadGameData()
    {
        if (System.IO.File.Exists(dataPath))
        {
            string json = System.IO.File.ReadAllText(dataPath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game data loaded from " + dataPath);
        }
        else
        {
            Debug.LogWarning("No save data found at " + dataPath);
        }
    }

    public void UpdatePlayerScore(int newScore)
    {
        if (currentPlayerData != null)
        {
            currentPlayerData.score = newScore;
        }
        else
        {
            Debug.LogWarning("Player not found");
        }
    }



    public void UnlockLevel(int level)
    {
        Debug.Log("Unlocking level " + level);
        if (currentPlayerData != null && !currentPlayerData.unlockedLevels.Contains(level))
        {
            currentPlayerData.currentLevel = level;
            currentPlayerData.unlockedLevels.Add(level);
            SaveGameData();
            Debug.Log("Unlocked level");

        }
    }


    public void EndGame(int score)
    {
        UpdatePlayerScore(score);
        SaveGameData();
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



}


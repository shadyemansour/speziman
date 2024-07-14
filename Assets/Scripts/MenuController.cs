using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Title screen script
/// </summary>
public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject enterName;
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject stats;
    [SerializeField] private TMP_Text nameText;
    private GameObject newPlayerObjects;
    private GameObject oldPlayerObjects;
    private HighscoreTable highscoreTable;
    private Button addNewPlayerButton;
    private Button existingPlayerButton;

    public Toggle skipIntroToggle;
    private TMP_InputField usernameInputField;
    private TMP_Text errorText;

    private List<Button> levelButtons;

    void Awake()
    {
        enterName.SetActive(true);  // Temporarily active to ensure it can be found if inactive at design time.
        newPlayerObjects = GameObject.FindGameObjectWithTag("NewPlayerMenu");
        oldPlayerObjects = GameObject.FindGameObjectWithTag("ExistingPlayerMenu");
        addNewPlayerButton = newPlayerObjects.GetComponentInChildren<Button>();
        existingPlayerButton = oldPlayerObjects.GetComponentInChildren<Button>();
        usernameInputField = GameObject.FindGameObjectWithTag("UsernameInputField").GetComponent<TMP_InputField>();
        usernameInputField.GetComponent<TextInputFilter>().SetOnEscape(GameObject.FindGameObjectWithTag("BackButton").GetComponent<Button>().onClick.Invoke);

        errorText = GameObject.Find("errorText").GetComponent<TMP_Text>();
        enterName.SetActive(false);

        levelSelect.SetActive(true);
        levelButtons = new List<Button>();
        InitializeLevelButtons();
        levelSelect.SetActive(false);
        stats.SetActive(true);
        highscoreTable = FindObjectOfType<HighscoreTable>();
        stats.SetActive(false);

        start.SetActive(true);
    }

    void Start()
    {
        if (GameManager.Instance.ShowLevelSelectOnLoad)
        {
            LevelSelectMenu();
            UpdateLevelAccess();
            GameManager.Instance.ShowLevelSelectOnLoad = false;

        }
    }

    private void InitializeLevelButtons()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("LevelButton");
        levelButtons.Clear();
        foreach (GameObject buttonObj in buttons)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                levelButtons.Add(button);
            }
            else
            {
                Debug.LogError("Button component not found on object with 'LevelButton' tag.");
            }
        }

        levelButtons.Sort((a, b) => a.name.CompareTo(b.name));

    }


    public void OnNewUserButtonClicked()
    {
        errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 0f);
        string username = usernameInputField.text;
        if (!string.IsNullOrEmpty(username))
        {
            bool success = GameManager.Instance.RegisterNewPlayer(username.ToLower());
            if (success)
            {
                Debug.Log("Registration successful.");
                UpdateLevelAccess();
                nameText.text = $"Hi {username}!";
                LevelSelectMenu();
                usernameInputField.text = "";

            }
            else
            {
                Debug.Log("Registration failed. Username already exists.");
                DisplayErrorMessages("Someone was faster than you! Try another name.");
                // Inform the user to choose a different username
            }
        }
        else
        {
            Debug.Log("Username cannot be empty.");
            DisplayErrorMessages("I'm sure you have a name!");
        }
    }

    // Call this method when the Login button is clicked
    public void OnExistingUserButtonClicked()
    {
        string username = usernameInputField.text;
        if (!string.IsNullOrEmpty(username))
        {
            bool found = GameManager.Instance.LoginExistingPlayer(username.ToLower());
            if (found)
            {
                Debug.Log("Login successful.");
                nameText.text = $"Hi {username}!";
                UpdateLevelAccess();
                LevelSelectMenu();
                usernameInputField.text = "";
            }
            else
            {
                Debug.Log("Login failed. User not found.");
                DisplayErrorMessages("Can't find you. Maybe you're new?");
                // Inform the user to choose a different username
            }
        }
        else
        {
            Debug.Log("Username cannot be empty.");
            DisplayErrorMessages("I'm sure you have a name!");

        }
    }

    public void UpdateLevelAccess()
    {
        int playerMaxLevel = GameManager.Instance.GetPlayerMaxLevel();

        for (int i = 0; i < levelButtons.Count; i++)
        {
            // Enable buttons up to the player's current max level
            levelButtons[i].interactable = i < playerMaxLevel;
        }
    }

    void DisplayErrorMessages(string message)
    {
        errorText.text = message;
        errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 1f);
    }






    //Navigation
    public void NewUserMenu()
    {
        usernameInputField.onSubmit.RemoveAllListeners();
        usernameInputField.onSubmit.AddListener(delegate { addNewPlayerButton.onClick.Invoke(); });
        SetMenuState(false, true, false, true, false, false);
    }

    public void ExistingUserMenu()
    {
        usernameInputField.onSubmit.RemoveAllListeners();
        usernameInputField.onSubmit.AddListener(delegate { existingPlayerButton.onClick.Invoke(); });
        SetMenuState(false, true, false, false, true, false);
    }

    public void LevelSelectMenu()
    {
        SetMenuState(false, false, true, false, false, false);
    }

    public void BackToMainMenu()
    {
        usernameInputField.text = "";
        errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 0f);
        SetMenuState(true, false, false, false, false, false);
    }

    public void BackToMainMenuLeaderboard()
    {

        SetMenuState(false, false, true, false, false, false);
    }

    public void ShowDetails()
    {
        SetMenuState(false, false, false, false, false, false);
    }
    public void ShowStats()
    {
        highscoreTable.PopulateTable();
        SetMenuState(false, false, false, false, false, true);
    }

    public void ChangePlayer()
    {
        GameManager.Instance.Logout();
        SetMenuState(true, false, false, false, false, false);
    }

    public void StartGame()
    {
        GameManager.Instance.LoadLevel(1, true);
    }

    public void StartLevel(int levelNum)
    {
        GameManager.Instance.LoadLevel(levelNum, true);
    }

    private void SetMenuState(bool showStart, bool showEnterName, bool showLevelSelect, bool showNewPlayerObjects, bool showOldPlayerObjects, bool showStats)
    {
        start.SetActive(showStart);
        enterName.SetActive(showEnterName);
        stats.SetActive(showStats);
        levelSelect.SetActive(showLevelSelect);
        newPlayerObjects.SetActive(showNewPlayerObjects);
        oldPlayerObjects.SetActive(showOldPlayerObjects);
    }
}
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
    public GameObject enterName;
    public GameObject levelSelect;
    private GameObject newPlayerObjects;
    private GameObject oldPlayerObjects;

    public Toggle skipIntroToggle;
    private TMP_InputField usernameInputField;
    private TMP_Text errorText;

    private List<Button> levelButtons;


    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject settingsCanvas;

    private void Awake()
    {
        enterName.SetActive(true);  // Temporarily active to ensure it can be found if inactive at design time.
        newPlayerObjects = GameObject.FindGameObjectWithTag("NewPlayerMenu");
        oldPlayerObjects = GameObject.FindGameObjectWithTag("ExistingPlayerMenu");
        usernameInputField = GameObject.FindGameObjectWithTag("UsernameInputField").GetComponent<TMP_InputField>();
        errorText = GameObject.Find("errorText").GetComponent<TMP_Text>();
        enterName.SetActive(false);
        
        levelSelect.SetActive(true);
        levelButtons = new List<Button>();
        InitializeLevelButtons();
        levelSelect.SetActive(false);
        
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

        
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettingsMenu);
        }
        else
        {
            Debug.LogError("Settings button is not assigned in the MenuController!");
        }

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("Settings canvas is not assigned in the MenuController!");
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
                LevelSelectMenu();
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
                UpdateLevelAccess();
                LevelSelectMenu();
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
        SetMenuState(false, true, false, true, false, false);
    }

    public void ExistingUserMenu()
    {
        SetMenuState(false, true, false, false, true, false);
    }

    public void LevelSelectMenu()
    {
        SetMenuState(false, false, true, false, false, false);
    }

    public void BackToMainMenu()
    {
        SetMenuState(true, false, false, false, false, false);
    }

    public void ShowDetails()
    {
        SetMenuState(false, false, false, false, false, false);
    }

    public void StartGame()
    {
        GameManager.Instance.LoadLevel(1, true);
    }

    public void StartLevel(int levelNum)
    {
        GameManager.Instance.LoadLevel(levelNum, true);
    }

    private void SetMenuState(bool showStart, bool showEnterName, bool showLevelSelect, bool showNewPlayerObjects, bool showOldPlayerObjects, bool showSettings)
    {
        start.SetActive(showStart);
        enterName.SetActive(showEnterName);
        levelSelect.SetActive(showLevelSelect);
        newPlayerObjects.SetActive(showNewPlayerObjects);
        oldPlayerObjects.SetActive(showOldPlayerObjects);
        settingsCanvas.SetActive(showSettings);
    }

    private void OpenSettingsMenu()     // alternatively with setMenuState
    {
        start.SetActive(false);
        enterName.SetActive(false);
        levelSelect.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void CloseSettingsMenu()    // alternatively with setMenuState
    {
        settingsCanvas.SetActive(false);    
        start.SetActive(true);
    }
}

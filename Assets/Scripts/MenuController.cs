using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Title screen script
/// </summary>
public class MenuController : MonoBehaviour
{

    public GameObject start;
    public GameObject choosePlayer;
    public GameObject details;
      public Toggle skipIntroToggle;

    public void ChoosePlayerMenu()
    {
        start.SetActive(false);
        choosePlayer.SetActive(true);
        details.SetActive(false);

    }

    public void BackToMainMenu()
    {
        start.SetActive(true);
        choosePlayer.SetActive(false);
        details.SetActive(false);
    }

    public void ShowDetails()
    {
        start.SetActive(false);
        choosePlayer.SetActive(false);
        details.SetActive(true);
    }

   public void StartGame()
    {
        if (skipIntroToggle.isOn)
        {
            GameManager.Instance.LoadLevel(1);
        }
        else
        {
            GameManager.Instance.LoadIntro();
        }
    }

    public void StartLevel(int levelNum) 
    {
        if (skipIntroToggle.isOn)
        {
            GameManager.Instance.LoadLevel(levelNum);
        }
        else
        {
            GameManager.Instance.LoadIntro();
        }
    }


}
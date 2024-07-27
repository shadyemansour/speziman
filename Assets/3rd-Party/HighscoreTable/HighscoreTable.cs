/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class HighscoreTable : MonoBehaviour
{

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;


    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        entryTemplate.gameObject.SetActive(false);
    }

    public void PopulateTable()
    {

        List<PlayerData> players = GameManager.Instance.gameData.players;
        List<(string playerName, int highScore)> playerScores = new List<(string playerName, int highScore)>();

        foreach (PlayerData player in players)
        {
            playerScores.Add((player.playerName, player.TotalScore));
        }

        playerScores.Sort((x, y) => y.highScore.CompareTo(x.highScore));

        highscoreEntryTransformList = new List<Transform>();
        foreach (var playerScore in playerScores)
        {
            CreateHighscoreEntryTransform(playerScore, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform((string playerName, int highScore) scoreData, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        entryTransform.Find("posText").GetComponent<Text>().text = GetOrdinal(rank);
        entryTransform.Find("scoreText").GetComponent<Text>().text = scoreData.highScore.ToString();
        entryTransform.Find("nameText").GetComponent<Text>().text = scoreData.playerName;

        bool isBackgroundActive = rank % 2 == 1;
        GameObject background = entryTransform.Find("background").gameObject;
        background.SetActive(isBackgroundActive);
        if (rank == 1)
        {
            HighlightFirstPlace(entryTransform);
        }
        else
        {
            Color textColor = isBackgroundActive ? Color.white : background.GetComponent<Image>().color;
            SetTextColors(entryTransform, textColor);
        }
        SetTrophyColor(rank, entryTransform.Find("trophy").GetComponent<Image>());

        transformList.Add(entryTransform);
    }

    private void SetTextColors(Transform entryTransform, Color color)
    {
        entryTransform.Find("posText").GetComponent<Text>().color = color;
        entryTransform.Find("scoreText").GetComponent<Text>().color = color;
        entryTransform.Find("nameText").GetComponent<Text>().color = color;
    }

    private string GetOrdinal(int rank)
    {
        switch (rank)
        {
            case 1: return "1ST";
            case 2: return "2ND";
            case 3: return "3RD";
            default: return rank + "TH";
        }
    }

    private void HighlightFirstPlace(Transform entryTransform)
    {
        Color newColor = UtilsClass.GetColorFromString("F6FF45");
        entryTransform.Find("posText").GetComponent<Text>().color = newColor;
        entryTransform.Find("scoreText").GetComponent<Text>().color = newColor;
        entryTransform.Find("nameText").GetComponent<Text>().color = newColor;
    }

    private void SetTrophyColor(int rank, Image trophy)
    {
        switch (rank)
        {
            case 1:
                trophy.color = UtilsClass.GetColorFromString("FFD200");
                trophy.gameObject.SetActive(true);
                break;
            default:
                trophy.gameObject.SetActive(false);
                break;
        }
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }

}

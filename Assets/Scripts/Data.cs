using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int score;

    public LevelData(int number, int score)
    {
        this.levelNumber = number;
        this.score = score;
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int currentLevel;
    public List<LevelData> levelsData = new List<LevelData>();

    public int TotalScore
    {
        get
        {
            int total = 0;
            foreach (var level in levelsData)
            {
                total += level.score;
            }
            return total;
        }
    }
}

[System.Serializable]
public class GameData
{
    public List<PlayerData> players = new List<PlayerData>();
}

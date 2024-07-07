using System.Collections.Generic;
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public int currentLevel;
    public List<int> unlockedLevels = new List<int>();
}

[System.Serializable]
public class GameData
{
    public List<PlayerData> players = new List<PlayerData>();
}

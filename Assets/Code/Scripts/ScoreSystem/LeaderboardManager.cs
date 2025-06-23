using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void SaveScore(int level, int newScore)
    {
        string key = $"Leaderboard_Level_{level}";
        string json = PlayerPrefs.GetString(key, "");

        LeaderboardData leaderboard;
        if (!string.IsNullOrEmpty(json))
            leaderboard = JsonUtility.FromJson<LeaderboardData>(json);
        else
            leaderboard = new LeaderboardData();

        leaderboard.scores.Add(newScore);
        leaderboard.scores.Sort((a, b) => b.CompareTo(a));

        if (leaderboard.scores.Count > 5)
            leaderboard.scores = leaderboard.scores.GetRange(0, 5);

        string newJson = JsonUtility.ToJson(leaderboard);
        PlayerPrefs.SetString(key, newJson);
        PlayerPrefs.Save();
    }

    public List<int> GetLeaderboardForLevel(int level)
    {
        string key = $"Leaderboard_Level_{level}";
        string json = PlayerPrefs.GetString(key, "");

        if (string.IsNullOrEmpty(json))
            return new List<int>();

        LeaderboardData leaderboard = JsonUtility.FromJson<LeaderboardData>(json);
        if (leaderboard == null || leaderboard.scores == null)
            return new List<int>();

        return leaderboard.scores;
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<int> scores = new List<int>();
}

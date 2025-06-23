using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardMenuManager : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> scoreTexts;
    public List<int> GetScoresForLevel(int level)
    {
        return LeaderboardManager.Instance.GetLeaderboardForLevel(level);
    }
}

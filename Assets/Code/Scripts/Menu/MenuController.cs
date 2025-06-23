using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject buttonContainer;
    public GameObject levelButtonContainer;
    public GameObject optionButtonContainer;
    public GameObject helpContainer;
    public GameObject leaderboardContainer;
    public GameObject levelLeaderBoardContainer;

    [SerializeField] private LeaderboardMenuManager leaderboardMenuManager;

    public List<TextMeshProUGUI> scoreTexts;

    void Start()
    {
        leaderboardMenuManager = GetComponent<LeaderboardMenuManager>();
        if (leaderboardMenuManager == null)
        {
            leaderboardMenuManager = GetComponentInChildren<LeaderboardMenuManager>();
        }
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        HideAll();
        buttonContainer.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        HideAll();
        levelButtonContainer.SetActive(true);
    }

    public void OnOptionButtonClicked()
    {
        HideAll();
        optionButtonContainer.SetActive(true);
    }

    public void OnHelpButtonClicked()
    {
        HideAll();
        helpContainer.SetActive(true);
    }

    public void OnLeaderboardButtonClicked()
    {
        HideAll();
        levelLeaderBoardContainer.SetActive(true);
    }

    public void OnLevelPlayClicked(int level)
    {
        HideAll();
        SceneManager.LoadScene(level);
    }

    public void OnLevelButtonClicked(int level)
    {
        List<int> scores = leaderboardMenuManager.GetScoresForLevel(level);
        Debug.Log($"List Score: {scores.Count}");
        UpdateLeaderboardUI(scores);
        levelLeaderBoardContainer.SetActive(false);
        leaderboardContainer.SetActive(true);
    }
    private void UpdateLeaderboardUI(List<int> scores)
    {
        for (int i = 0; i < scoreTexts.Count; i++)
        {
            if (scoreTexts[i] == null) continue;

            if (i < scores.Count)
                scoreTexts[i].text = $"{scores[i]}";
            else
                scoreTexts[i].text = "";
        }
    }

    private void HideAll()
    {
        buttonContainer.SetActive(false);
        levelButtonContainer.SetActive(false);
        optionButtonContainer.SetActive(false);
        helpContainer.SetActive(false);
        leaderboardContainer.SetActive(false);
        levelLeaderBoardContainer.SetActive(false);
    }
}

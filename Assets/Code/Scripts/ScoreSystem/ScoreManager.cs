using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-100)]
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public TMP_Text scoreText;
    public TMP_Text multiplierText;
    public CanvasGroup multiplierGroup;

    public int currentMultiplier = 1;
    public int highScore = 0;
    public TMP_Text highScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (multiplierGroup != null)
        {
            multiplierGroup.alpha = 0f;
            multiplierGroup.interactable = false;
            multiplierGroup.blocksRaycasts = false;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void SetMultiplier(int multiplier)
    {
        currentMultiplier = multiplier;
        UpdateMultiplierText();
        FadeMultiplier(currentMultiplier > 1);
    }

    public string GetScoreText()
    {
        return "Score: " + score;
    }

    public int GetScoreForSave()
    {
        return score;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateMultiplierText()
    {
        if (multiplierText != null)
            multiplierText.text = "Trick multiplier: x" + currentMultiplier;
    }

    void FadeMultiplier(bool show)
    {
        if (multiplierGroup == null) return;

        multiplierGroup.alpha = show ? 1f : 0f;
        multiplierGroup.interactable = show;
        multiplierGroup.blocksRaycasts = show;
    }

    public void AddTrickScore(int amount)
    {
        AddScore(amount);
    }

    public void CheckAndSaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("HighScore", score);
        PlayerPrefs.Save();
    }

    public void LoadScore()
    {
        score = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreText();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public TMP_Text scoreText;

    [Header("Speed Scoring")]
    public float speedThreshold = 10f;
    public float speedMultiplier = 1.5f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void AddSpeedScore(float speed)
    {
        if (speed > speedThreshold)
        {
            int bonus = Mathf.RoundToInt((speed - speedThreshold) * speedMultiplier);
            AddScore(bonus);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}


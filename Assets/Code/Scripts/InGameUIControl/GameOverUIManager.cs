using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance;

    [Header("Canvas")]
    [SerializeField] private GameObject canvas;

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject crashPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI crashScoreText;
    [SerializeField] private TextMeshProUGUI winScoreText;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        canvas.SetActive(false);

        // Hide both panels on start
        winPanel.SetActive(false);
        crashPanel.SetActive(false);
    }

    public void ShowCrashUI()
    {
        Time.timeScale = 0f;
        crashScoreText.text = ScoreManager.Instance.GetScoreText();
        canvas.SetActive(true);
        crashPanel.SetActive(true);
        winPanel.SetActive(false);
    }

    public void ShowWinUI()
    {
        winScoreText.text = ScoreManager.Instance.GetScoreText();
        canvas.SetActive(true);
        winPanel.SetActive(true);
        crashPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    // Button handlers
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene >= SceneManager.sceneCountInBuildSettings)
            nextScene = 0;

        SceneManager.LoadScene(nextScene);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}

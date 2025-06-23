using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            LeaderboardManager.Instance.SaveScore(SceneManager.GetActiveScene().buildIndex, ScoreManager.Instance.GetScoreForSave());
            CrashHandler.Instance.TriggerCrash();
        }
    }
}

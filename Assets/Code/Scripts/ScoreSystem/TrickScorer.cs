using UnityEngine;

public class TrickScorer : MonoBehaviour
{
    public int trickScore = 50;
    public void OnTrickPerformed() //sd ham nay khi perform trick thanh cong
    {
        ScoreManager.Instance.AddScore(trickScore);
    }
}

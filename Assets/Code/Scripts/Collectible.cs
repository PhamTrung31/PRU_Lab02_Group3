using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 50;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
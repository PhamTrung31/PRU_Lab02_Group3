using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpeedScorer : MonoBehaviour
{
    private Rigidbody rb;
    public float checkInterval = 1.0f; //check speed moi 1.0f giay
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            float speed = rb.velocity.magnitude;
            ScoreManager.Instance.AddSpeedScore(speed);
            timer = 0f;
        }
    }
}

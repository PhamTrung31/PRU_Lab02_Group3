using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashRock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CrashHandler.Instance.TriggerCrash();
        }
    }
}

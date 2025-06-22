using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashHandler : MonoBehaviour
{
    public static CrashHandler Instance;

    [SerializeField] float delayTime = 1f;
    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] AudioClip crashSFX;

    private bool hasCrashed = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerCrash()
    {
        if (hasCrashed) return;

        hasCrashed = true;

        if (crashEffect != null) crashEffect.Play();
        if (crashSFX != null) AudioSource.PlayClipAtPoint(crashSFX, Camera.main.transform.position);

        Invoke(nameof(ReloadScene), delayTime);
    }

    private void ReloadScene()
    {
        Debug.Log($"Reloading scene: {SceneManager.GetActiveScene().name}");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

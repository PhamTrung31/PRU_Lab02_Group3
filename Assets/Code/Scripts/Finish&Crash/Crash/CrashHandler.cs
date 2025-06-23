using UnityEngine;

public class CrashHandler : MonoBehaviour
{
    public static CrashHandler Instance;

    [SerializeField] ParticleSystem crashEffect;
    [SerializeField] AudioClip crashSFX;
    [SerializeField] float delayTime = 1f;

    private bool hasCrashed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("CrashHandler initialized");
        }
        else
        {
            Debug.LogWarning("Duplicate CrashHandler destroyed");
            Destroy(gameObject);
        }
    }

    public void TriggerCrash()
    {
        if (hasCrashed) return;
        hasCrashed = true;

        if (crashEffect != null) crashEffect.Play();
        if (crashSFX != null) AudioSource.PlayClipAtPoint(crashSFX, Camera.main.transform.position);

        Invoke("ShowCrashUI", delayTime);
    }

    private void ShowCrashUI()
    {
        GameOverUIManager.Instance.ShowCrashUI();
    }

    public void TriggerWin()
    {
        if (hasCrashed) return;
        hasCrashed = true;

        Invoke("ShowWinUI", delayTime);
    }

    private void ShowWinUI()
    {
        GameOverUIManager.Instance.ShowWinUI();
    }
}

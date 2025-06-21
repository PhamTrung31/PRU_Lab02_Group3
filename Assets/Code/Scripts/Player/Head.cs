using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Head : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;

    [SerializeField] ParticleSystem crashEffect;

    bool hasCrashed = false;

    [SerializeField] AudioClip crashSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground" && !hasCrashed)
        {
            hasCrashed = true;
            crashEffect.Play();
            GetComponent<AudioSource>().PlayOneShot(crashSFX);
            Invoke("ReloadScene", loadDelay);
        }
    }
    void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}

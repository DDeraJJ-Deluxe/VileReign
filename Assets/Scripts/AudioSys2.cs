using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioSys2 : MonoBehaviour
{
    public AudioClip[] music; 
    private static AudioSource audioClips;
    private static AudioSys2 instance;
    public int musicIndex;
    private static int currentIndex = -1;
    private float fadeDuration = 1.0f;

    void Awake() {
        audioClips = GetComponent<AudioSource>();
        instance = this;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (musicIndex == -1) {
                StartCoroutine(FadeOut());
                currentIndex = musicIndex;
            } else if (currentIndex != musicIndex) {
                StartCoroutine(FadeOutAndFadeIn());
                currentIndex = musicIndex;
            }
        }
    }

    IEnumerator FadeOut() {
        while (audioClips.volume > 0) {
            audioClips.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioClips.Stop();
    }

    IEnumerator FadeOutAndFadeIn() {
        while (audioClips.volume > 0) {
            audioClips.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        audioClips.Stop();
        audioClips.clip = music[musicIndex];
        audioClips.Play();
        while (audioClips.volume < 1) {
            audioClips.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSys2 : MonoBehaviour
{
    public AudioClip[] music; 
    private AudioSource audioClips; 

    void Awake() {
        audioClips = GetComponent<AudioSource>();
        audioClips.clip = music[0];
        audioClips.Play();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
                audioClips.clip = music[1];
                audioClips.Play();
        }
    }

    public void newMusic(int clipIndex) {

        audioClips.Stop();

        audioClips.clip = music[clipIndex];
        audioClips.Play();
    }
}
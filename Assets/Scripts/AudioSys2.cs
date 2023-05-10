using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSys2 : MonoBehaviour
{
    public AudioClip[] music; 
    private static AudioSource audioClips;
    private static AudioSys2 instance;
    public int musicIndex;
    private static int currentIndex = -1;

    void Awake() {
        audioClips = GetComponent<AudioSource>();
        instance = this;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (currentIndex != musicIndex) {
                // Stop the previous music track
                audioClips.Stop();

                // Start the current music track
                audioClips.clip = music[musicIndex];
                audioClips.Play();
                currentIndex = musicIndex;
            }
        }
    }
}
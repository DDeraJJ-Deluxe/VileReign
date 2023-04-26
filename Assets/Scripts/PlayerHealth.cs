using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    
    public int maxHealth = 10;
    public int health;

    void Start() {
        health = maxHealth;
    }

    void Update() {
        if (gameObject.transform.position.y < -7 ){
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}

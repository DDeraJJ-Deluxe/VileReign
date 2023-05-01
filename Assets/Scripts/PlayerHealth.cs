using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    
    public int maxHealth = 100;
    public int health;

    public HealthBar healthBar;

    void Start() {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update() {
        if (gameObject.transform.position.y < -7 ){
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void TakeDamage(int damage) {
        health -= damage;

        healthBar.SetHealth(health);
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}

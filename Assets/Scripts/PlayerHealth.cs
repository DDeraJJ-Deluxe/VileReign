using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {
    
    public int maxHealth = 100;
    public int health;

    public Animator animator;

    public HealthBar healthBar;
    public PlayerController player;

    void Start() {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update() {
        if (gameObject.transform.position.y < -7 ){
            SceneManager.LoadScene("SampleScene");
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage) {

        if (player.isInvincible) {
            return;
        }

        health -= damage;

        StartCoroutine(DamageAnimation());

        healthBar.SetHealth(health);
        if (health <= 0) {
            Die();
        }
    }

    void Die() {
        animator.SetBool("isDead", true);
        this.enabled = false;
    }

    IEnumerator DamageAnimation()
	{
		SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

		for (int i = 0; i < 3; i++)
		{
			foreach (SpriteRenderer sr in srs)
			{
				Color c = sr.color;
				c.a = 0;
				sr.color = c;
			}

			yield return new WaitForSeconds(.1f);

			foreach (SpriteRenderer sr in srs)
			{
				Color c = sr.color;
				c.a = 1;
				sr.color = c;
			}

			yield return new WaitForSeconds(.1f);
		}
	}
}
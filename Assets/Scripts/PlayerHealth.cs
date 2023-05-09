using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour {
    
    public int maxHealth = 100;
    public int health;
    public int defense = 20;

    [SerializeField] public TextMeshProUGUI defenseDisplay;

    public Animator animator;

    public HealthBar healthBar;
    public PlayerController player;

    void Start() {
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update() {
        defenseDisplay.text = defense.ToString();
        if (gameObject.transform.position.y < -10 ){
            TakeDamage(maxHealth);
        }
    }

    public void TakeDamage(int damage) {

        if (player.isInvincible) {
            return;
        }

        float actual_damage_float = (float)damage * ((float)(120 - defense) / 100.0f);
        int actual_damage = (int)(actual_damage_float + 0.5f); // round to nearest integer
        if (actual_damage == 0) {
            actual_damage = 1;
        }
        health -= actual_damage;
        StartCoroutine(DamageAnimation());

        healthBar.SetHealth(health);
        if (health <= 0) {
            Die();
            StartCoroutine(DeathAnimation());
        }
    }

    public void Heal() {
        health = maxHealth;
        healthBar.SetHealth(maxHealth);
    }

    void Die() {
        animator.SetBool("isDead", true);
        player.isDead = true;
        this.enabled = false;
    }

    IEnumerator DeathAnimation() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SampleScene");
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

    public void IncreaseDefense(int increasedDefense) {
        defense += increasedDefense;
    }
}
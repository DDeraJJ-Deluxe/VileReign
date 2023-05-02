using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed;

    public Transform playerTransform;
    public bool isChasing;
    public float chaseDistance;

    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 0.4f;
    public int attackDamage = 12;
    public float attackRate = 1.5f;
    float nextAttackTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        healthBar.gameObject.SetActive(false);
    }

    void Update() {
        if (isChasing) {
            animator.SetFloat("Speed", Mathf.Abs(Input.GetAxisRaw("Horizontal")));
            if (transform.position.x > playerTransform.position.x) {
                transform.localScale = new Vector3(6.26492f, 6.26492f, 6.26492f);
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (transform.position.x < playerTransform.position.x) {
                transform.localScale = new Vector3(-6.26492f, 6.26492f, 6.26492f);
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > chaseDistance) {
                isChasing = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }

            if (distanceToPlayer < 1f) {
                if (Time.time >= nextAttackTime) {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        } else {
            if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance) {
                isChasing = true;
            }
        }
    }

    void Attack() {
        animator.SetTrigger("Attack");

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach(Collider2D player in hitPlayer) {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }
}

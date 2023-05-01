using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    int currentHealth;

    public float moveSpeed;

    public Transform playerTransform;
    public bool isChasing;
    public float chaseDistance;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    void Update() {
        if (isChasing) {
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
            }
        } else {
            if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance) {
                isChasing = true;
            }
        }
    }
}

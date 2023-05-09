using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSkeleton : MonoBehaviour {
    public Animator animator;
    public GameObject projectilePrefab;
    public Transform launchPoint;

    public int maxHealth = 60;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 1.5f;

    public Transform playerTransform;
    public bool isAttacking;
    public bool isFacingRight = false;

    public float attackDistance = 10f;
    public float attackRate = 0.88f;
    float nextAttackTime = 0f;

    private bool isDead = false;
    public int expDropped = 45;

    public PlayerController playerController;


    void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
        playerController.GainExp(expDropped);
        isDead = true;
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        healthBar.gameObject.SetActive(false);
    }

    void Update() {
        if (isAttacking) {

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackDistance && distanceToPlayer > 6f) { //6-10
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
                if (Time.time >= nextAttackTime) {
                    animator.SetTrigger("Attack");
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }

            if (distanceToPlayer <= 6f) { // <6
                if (transform.position.x > playerTransform.position.x) {
                    isFacingRight = false;
                    transform.localScale = new Vector3(6.26492f, 6.26492f, 6.26492f);
                    transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                }
                if (transform.position.x < playerTransform.position.x) {
                    isFacingRight = true;
                    transform.localScale = new Vector3(-6.26492f, 6.26492f, 6.26492f);
                    transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                }
                if (Time.time >= nextAttackTime) {
                    animator.SetTrigger("Attack");
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }

            if (distanceToPlayer > 11f) { // >11
                isAttacking = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }
        } else {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackDistance) { // <10
                isAttacking = true;
            }
            if (distanceToPlayer <= 11.5f) { // <11
                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                if (transform.position.x > playerTransform.position.x) {
                    isFacingRight = false;
                    transform.localScale = new Vector3(6.26492f, 6.26492f, 6.26492f);
                    transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                }
                if (transform.position.x < playerTransform.position.x) {
                    isFacingRight = true;
                    transform.localScale = new Vector3(-6.26492f, 6.26492f, 6.26492f);
                    transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    public void Attack() {
        if (!isDead) {
            Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        }
    }
}
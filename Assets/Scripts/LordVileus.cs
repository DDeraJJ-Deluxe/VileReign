using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordVileus : MonoBehaviour {

    public Animator animator;
    public Rigidbody2D rb;

    public GameObject projectilePrefab;
    public Transform launchPoint;

    public int maxHealth = 1500;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 3.5f;

    public bool isFacingRight = false;

    public float attackDistance = 12f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 0.5f;
    public int attackDamage = 20;

    public float attackRate = 1f;
    float nextAttackTime = 0f;
    public bool isAttacking = false;

    public float castRate = 2f;
    private float nextCastTime = 0f;
    public bool isCasting = false;

    public float summonRate = 0.05f;
    private float nextSummonTime = 0f;
    public bool isSummoning = false;

    private bool isDead = false;
    public int expDropped = 1000;

    public PlayerController playerController;
    public Transform playerTransform;

    public Transform boundary;

    public GameObject[] enemyPrefabs;
    public Transform enemySpawnerA;
    public Transform enemySpawnerB;
    public Transform enemySpawnerC;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        healthBar.gameObject.SetActive(false);
        boundary.GetComponent<Collider2D>().enabled = false;
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
        boundary.GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        healthBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance) {
            healthBar.gameObject.SetActive(true);
            boundary.GetComponent<Collider2D>().enabled = true;
            if (!isSummoning && !isCasting) {
                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }
            if (transform.position.x > playerTransform.position.x && !isAttacking) {
                isFacingRight = false;
                transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
            }
            if (transform.position.x < playerTransform.position.x && !isAttacking) {
                isFacingRight = true;
                transform.localScale = new Vector3(-3.6f, 3.6f, 3.6f);
            }
            if (Time.time >= nextSummonTime && !isAttacking && !isCasting) {
                animator.SetTrigger("Summon");
                nextSummonTime = Time.time + 1f / summonRate;
            } 
            if (distanceToPlayer > 1f && Time.time >= nextCastTime && !isAttacking && !isSummoning) {
                animator.SetTrigger("Cast");
                nextCastTime = Time.time + 1f / castRate;
            } 
            if (distanceToPlayer <= 1f && Time.time >= nextAttackTime && !isSummoning && !isCasting) {
                animator.SetTrigger("Attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
            if (isSummoning || isCasting) {
                rb.velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }
            if (isAttacking) {
                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }
        } else {
            healthBar.gameObject.SetActive(false);
        }
    }

    public void Cast() {
        if (!isDead) {
            Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        }
    }

    public void Attack() {
        if (!isDead) {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
    }

    public void Summon() {
        if (!isDead) {
            int rand = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawnA = enemyPrefabs[rand];
            rand = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawnB = enemyPrefabs[rand];
            rand = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawnC = enemyPrefabs[rand];

            Instantiate(enemyToSpawnA, enemySpawnerA.position, Quaternion.identity);
            Instantiate(enemyToSpawnB, enemySpawnerB.position, Quaternion.identity);
            Instantiate(enemyToSpawnC, enemySpawnerC.position, Quaternion.identity);
        }
    }

    public void SetCasting() {
        isCasting = !isCasting;
    }

    public void SetAttacking() {
        isAttacking = !isAttacking;
    }

    public void SetSummoning() {
        isSummoning = !isSummoning;
    }
}

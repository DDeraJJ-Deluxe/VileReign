using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidReaper : MonoBehaviour {

    public Animator animator;
    public GameObject projectilePrefab;

    public int maxHealth = 750;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 3.25f;

    public Transform playerTransform;
    public bool isAttacking;
    public bool isFacingRight = false;

    public float attackDistance = 15f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 1.5f;
    public int attackDamage = 15;
    public float attackRate = 0.5f;
    private float nextAttackTime = 0f;

    public float castRate = 10f;
    private float nextCastTime = 0f;
    public bool isCasting = false;

    private bool isDead = false;
    public int expDropped = 250;

    public PlayerController playerController;
    public PlayerHealth playerHealth;
    public SpriteRenderer sr;

    public Transform boundary;
    public Transform doubleJumpLocation;
    public GameObject doubleJumpPrefab;

    void Start() {
        healthBar.gameObject.SetActive(false);
        boundary.GetComponent<Collider2D>().enabled = false;
        sr = GetComponent<SpriteRenderer>();
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
        playerHealth.Heal();
        isDead = true;
        animator.SetBool("isDead", true);
        boundary.GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        healthBar.gameObject.SetActive(false);
        RevealDoubleJump();

    }

    void Update() {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance) {
            healthBar.gameObject.SetActive(true);
            boundary.GetComponent<Collider2D>().enabled = true;
            if ((float)currentHealth <= (float)(maxHealth * 0.5f)) {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
                if (transform.position.x > playerTransform.position.x && !isAttacking) {
                    transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                }
                if (transform.position.x < playerTransform.position.x && !isAttacking) {
                    transform.localScale = new Vector3(-3.6f, 3.6f, 3.6f);
                }
                if (Time.time >= nextCastTime) {
                    animator.SetTrigger("Cast");
                    nextCastTime = Time.time + 1f / castRate;
                } 
                if (!isCasting) {
                    if (transform.position.x > playerTransform.position.x && !isAttacking) {
                        animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                        transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                    }
                    if (transform.position.x < playerTransform.position.x && !isAttacking) {
                        animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                        transform.localScale = new Vector3(-3.6f, 3.6f, 3.6f);
                        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                    }
                    if (distanceToPlayer <= 2.5f) { // <1
                        if (Time.time >= nextAttackTime) {
                            Attack();
                            nextAttackTime = Time.time + 1f / attackRate;
                        }
                    }
                }
            } else {
                if (transform.position.x > playerTransform.position.x && !isAttacking) {
                    animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                    transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                    transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                }
                if (transform.position.x < playerTransform.position.x && !isAttacking) {
                    animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                    transform.localScale = new Vector3(-3.6f, 3.6f, 3.6f);
                    transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                }
                if (distanceToPlayer <= 2.5f) { // <1
                    if (Time.time >= nextAttackTime) {
                        Attack();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
            }
        }
        if (distanceToPlayer > attackDistance) {
            healthBar.gameObject.SetActive(false);
        }
    }

    public void Attack() {
        isAttacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(DelayForDamage());
    }

    private IEnumerator DelayForDamage() {
        yield return new WaitForSeconds(0.4f);
        if (!isDead) {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    void RevealDoubleJump() {
        Instantiate(doubleJumpPrefab, doubleJumpLocation.position, Quaternion.identity);
    }

    void CreateProjectile() {
        Vector3 spawnPosition = playerTransform.position + new Vector3(0f, 8f, 0f);
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    }

    public void setIsCasting() {
        isCasting = !isCasting;
    }
}

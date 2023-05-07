using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidReaper : MonoBehaviour {

    public Animator animator;

    public int maxHealth = 750;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 4f;

    public Transform playerTransform;
    public bool isAttacking;
    public bool isFacingRight = false;

    public float attackDistance = 15f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 1.5f;
    public int attackDamage = 15;
    public float attackRate = 1f;
    float nextAttackTime = 0f;

    private bool isDead = false;
    public int expDropped = 250;

    public PlayerController playerController;
    public SpriteRenderer sr;

    void Start() {
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
        isDead = true;
        animator.SetBool("isDead", true);
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        healthBar.gameObject.SetActive(false);
    }

    void Update() {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance) {
            /*
            if (transform.position.x > playerTransform.position.x) {
                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (transform.position.x < playerTransform.position.x) {
                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                transform.localScale = new Vector3(-3.6f, 3.6f, 3.6f);
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }*/
            if (distanceToPlayer <= 2f) { // <1
                if (Time.time >= nextAttackTime) {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }
    }

    public void Attack() {
        animator.SetTrigger("Attack");
        StartCoroutine(DelayForDamage());
    }

    public void Cast() {
        animator.SetTrigger("Cast");
    }

    private IEnumerator DelayForDamage() {
        yield return new WaitForSeconds(0.5f);
        if (!isDead) {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
    }
}

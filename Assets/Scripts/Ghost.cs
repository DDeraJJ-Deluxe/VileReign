using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Animator animator;
    public GameObject projectilePrefab;
    public Transform launchPoint;

    public int maxHealth = 80;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 3.5f;

    public Transform playerTransform;
    public bool isAttacking;
    public bool isFacingRight = false;

    public float attackDistance = 10f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 0.4f;
    public int attackDamage = 8;
    public float attackRate = 1f;
    float nextAttackTime = 0f;
    float nextInvisibleTime = 0f;
    public bool isInvisible = false;

    private bool isDead = false;
    public int expDropped = 75;

    public PlayerController playerController;
    public SpriteRenderer sr;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update() {
        if (isAttacking) {

            if (isInvisible) {
                animator.SetFloat("Speed", Mathf.Abs(4.75f));
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, 4.75f * Time.deltaTime);
            } else {
                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackDistance && distanceToPlayer > 1f && !isInvisible) {
                if (transform.position.x > playerTransform.position.x) {
                    isFacingRight = false;
                    transform.localScale = new Vector3(6.26492f, 6.26492f, 6.26492f);
                }
                if (transform.position.x < playerTransform.position.x) {
                    isFacingRight = true;
                    transform.localScale = new Vector3(-6.26492f, 6.26492f, 6.26492f);
                }
                if (Time.time >= nextInvisibleTime) {
                    StartCoroutine(FadeOutIn());
                    nextInvisibleTime = Time.time + 20f;
                }
                if (Time.time >= nextAttackTime) {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }

            if (distanceToPlayer <= 1f && !isInvisible) { // <1
                if (transform.position.x > playerTransform.position.x) {
                    isFacingRight = false;
                    transform.localScale = new Vector3(6.26492f, 6.26492f, 6.26492f);
                }
                if (transform.position.x < playerTransform.position.x) {
                    isFacingRight = true;
                    transform.localScale = new Vector3(-6.26492f, 6.26492f, 6.26492f);
                }
                if (Time.time >= nextAttackTime) {
                    Attack2();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }

            if (distanceToPlayer > attackDistance) { // >12
                isAttacking = false;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }
        } else {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackDistance) { // <12
                if (Time.time >= nextInvisibleTime) {
                    StartCoroutine(FadeOutIn());
                    nextInvisibleTime = Time.time + 20f;
                }
                isAttacking = true;
            }
        }
    }

    public void Attack() {
        animator.SetTrigger("Attack");
        StartCoroutine(DelayForAttack());
    }

    public void Attack2() {
        animator.SetTrigger("Attack");
        StartCoroutine(DelayForDamage());
    }

    private IEnumerator DelayForAttack() {
        yield return new WaitForSeconds(1.8f);
        if (!isDead || !isInvisible) {
            Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        }
    }

    private IEnumerator DelayForDamage() {
        yield return new WaitForSeconds(0.5f);
        if (!isDead || !isInvisible) {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
    }

    private IEnumerator FadeOutIn() {
        isInvisible = true;
        GetComponent<Collider2D>().enabled = false;
        float fadeOutDuration = 0.8f; // Divide the total duration in half to get the time to fade out and back in
        float alpha = 1f;
        while (alpha > 0f) // Fade out
        {
            alpha -= Time.deltaTime / fadeOutDuration; // Decrease the alpha value over time
            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
            yield return null; // Wait for the next frame
        }
        healthBar.gameObject.SetActive(false);
        alpha = 0f; // Set the starting alpha value to 0 for the fade in
        yield return new WaitForSeconds(5f); // Wait for the duration of the fade out

        GetComponent<Collider2D>().enabled = true;
        isInvisible = false;
        healthBar.gameObject.SetActive(true);
        while (alpha < 1f) // Fade in
        {
            alpha += Time.deltaTime / fadeOutDuration; // Increase the alpha value over time
            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
            yield return null; // Wait for the next frame
        }
        Color finalColor = sr.color;
        finalColor.a = 1f;
        sr.color = finalColor; // Apply the final color to the sprite
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenHero : MonoBehaviour {

    public Animator animator;
    //public GameObject projectilePrefab;

    public int maxHealth = 1000;
    int currentHealth;
    public HealthBar healthBar;

    public float moveSpeed = 4f;

    public Transform playerTransform;
    public bool isAttacking;
    public bool isFacingRight = false;

    public float attackDistance = 10f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    public float attackRange = 0.5f;
    public int attackDamage = 20;
    public float attackRate = 1f;
    private float nextAttackTime = 0f;
    
    public float strikeRate = 0.1f;
    private float nextStrikeTime = 0f;
    public bool isStriking = false;
    public int strikeDamage = 15;
    public float strikeSpeed = 20f;
    //private float strikeTimeRemaining = 0f;
    //public float strikeDuration = 0.1f;

    private bool isDead = false;
    public int expDropped = 500;

    public bool dodging = false;
    private float dodgeTimeRemaining = 0f;
    public float dodgeDuration = 0.125f;
    public float dodgeSpeed = 24f;
    public float dodgeRate = 0.2f;
    float nextDodgeTime = 0f;
    public bool isInvincible = false;

    public PlayerController playerController;
    public PlayerHealth playerHealth;
    public SpriteRenderer sr;

    public Transform boundary;
    public Transform swordBeamLocation;
    public GameObject swordBeamPrefab;

    Rigidbody2D rb;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        healthBar.gameObject.SetActive(false);
        boundary.GetComponent<Collider2D>().enabled = false;
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage) {
        if (isInvincible) {
            return;
        }
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
        RevealSwordBeam();

    }

    void Update() {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance) {
            healthBar.gameObject.SetActive(true);
            boundary.GetComponent<Collider2D>().enabled = true;

            if ((float)currentHealth <= (float)(maxHealth * 0.5f)) {
                if (transform.position.x > playerTransform.position.x && !isAttacking && !isStriking) {
                    transform.localScale = new Vector3(3.6f, 3.6f, 3.6f);
                }
                if (transform.position.x < playerTransform.position.x && !isAttacking && !isStriking) {
                    transform.localScale = new Vector3(-3.6f, 3.6f, 3.6f);
                }
                if (Time.time >= nextStrikeTime) {
                    isStriking = true;
                    nextStrikeTime = Time.time + 1f / strikeRate;
                } 
                if (isStriking) { // Duration during dodge
                    if (transform.position.x < playerTransform.position.x) { 
                        animator.SetFloat("Speed", Mathf.Abs(strikeSpeed * 3f));
                        rb.velocity = new Vector2(strikeSpeed * 3f, rb.velocity.y);
                    }
                    if (transform.position.x > playerTransform.position.x) {
                        animator.SetFloat("Speed", Mathf.Abs(strikeSpeed * 3f));
                        rb.velocity = new Vector2(-strikeSpeed * 3f, rb.velocity.y);
                    }
                    StartCoroutine(DelayForStriking());
                }
                if (!isStriking) {
                    if (dodging) {
                        if (transform.position.x < playerTransform.position.x && !isAttacking) { 
                            animator.SetFloat("Speed", Mathf.Abs(dodgeSpeed));
                            rb.velocity = new Vector2(-dodgeSpeed, rb.velocity.y); // Apply dodge speed to player if dodging
                        }
                        if (transform.position.x > playerTransform.position.x && !isAttacking) {
                            animator.SetFloat("Speed", Mathf.Abs(dodgeSpeed));
                            rb.velocity = new Vector2(dodgeSpeed, rb.velocity.y);
                        }
                    } else {
                        if (!isDead) {
                            if (transform.position.x > playerTransform.position.x && !isAttacking) {
                                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                                transform.localScale = new Vector3(-3.942011f, 3.942011f, 3.942011f);
                                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                            }
                            if (transform.position.x < playerTransform.position.x && !isAttacking) {
                                animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                                transform.localScale = new Vector3(3.942011f, 3.942011f, 3.942011f);
                                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                            }
                        }
                    }

                    if (dodging) { // Duration during dodge
                        dodgeTimeRemaining -= Time.deltaTime;
                        if (dodgeTimeRemaining <= 0) {
                            dodging = false;
                        }
                    }

                    if (distanceToPlayer <= 1f) { // <1
                        if (Time.time >= nextDodgeTime) {
                            if (!isDead) { // If left shift key is pressed while moving horizontally
                                dodging = true;
                                dodgeTimeRemaining = dodgeDuration;
                                StartCoroutine(FadeOutIn(dodgeDuration));
                                nextDodgeTime = Time.time + 1f / dodgeRate;
                            }
                        }
                        if (Time.time >= nextAttackTime) {
                            animator.SetTrigger("Attack");
                            nextAttackTime = Time.time + 1f / attackRate;
                        }
                    }
                }
            } else {
                if (dodging) {
                    if (transform.position.x < playerTransform.position.x && !isAttacking) { 
                        animator.SetFloat("Speed", Mathf.Abs(dodgeSpeed));
                        rb.velocity = new Vector2(-dodgeSpeed, rb.velocity.y); // Apply dodge speed to player if dodging
                    }
                    if (transform.position.x > playerTransform.position.x && !isAttacking) {
                        animator.SetFloat("Speed", Mathf.Abs(dodgeSpeed));
                        rb.velocity = new Vector2(dodgeSpeed, rb.velocity.y);
                    }
                } else {
                    if (!isDead) {
                        if (transform.position.x > playerTransform.position.x && !isAttacking) {
                            animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                            transform.localScale = new Vector3(-3.942011f, 3.942011f, 3.942011f);
                            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                        }
                        if (transform.position.x < playerTransform.position.x && !isAttacking) {
                            animator.SetFloat("Speed", Mathf.Abs(moveSpeed));
                            transform.localScale = new Vector3(3.942011f, 3.942011f, 3.942011f);
                            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                        }
                    }
                }

                if (dodging) { // Duration during dodge
                    dodgeTimeRemaining -= Time.deltaTime;
                    if (dodgeTimeRemaining <= 0) {
                        dodging = false;
                    }
                }

                if (distanceToPlayer <= 1f) { // <1
                    if (Time.time >= nextDodgeTime) {
                        if (!isDead) { // If left shift key is pressed while moving horizontally
                            dodging = true;
                            dodgeTimeRemaining = dodgeDuration;
                            StartCoroutine(FadeOutIn(dodgeDuration));
                            nextDodgeTime = Time.time + 1f / dodgeRate;
                        }
                    }
                    if (Time.time >= nextAttackTime) {
                        animator.SetTrigger("Attack");
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
        if (!isDead) {
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            }
        }
        StartCoroutine(DelayForDamage());
    }

    private IEnumerator DelayForDamage() {
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }

    private IEnumerator DelayForStriking() {
        yield return new WaitForSeconds(0.1f);
        isStriking = false;
    }

    void RevealSwordBeam() {
        Instantiate(swordBeamPrefab, swordBeamLocation.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (isStriking) {
            if (collision.gameObject.GetComponent<PlayerHealth>() != null) {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(strikeDamage);
            } 
        }
    }

    /* Fades out and in for a given duration */
    IEnumerator FadeOutIn(float duration) {
        isInvincible = true;
        float fadeOutDuration = duration / 2f; // Divide the total duration in half to get the time to fade out and back in
        float alpha = 1f;
        while (alpha > 0.5f) // Fade out
        {
            alpha -= Time.deltaTime / fadeOutDuration; // Decrease the alpha value over time
            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
            yield return null; // Wait for the next frame
        }

        alpha = 0.5f; // Set the starting alpha value to 0 for the fade in
        yield return new WaitForSeconds(fadeOutDuration); // Wait for the duration of the fade out

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
        isInvincible = false;
    }
}

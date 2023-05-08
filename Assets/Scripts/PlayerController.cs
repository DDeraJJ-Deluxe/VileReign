using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 6f;  // Player walk speed

    /* Variable jump height */
    public float buttonTime = 0.5f; // Limit of jump button pressed
    public float jumpHeight = 3f; // Max height of jump
    public float cancelRate = 100; // For jump-cancelling
    float jumpTime; // Store time during jump
    bool jumping; // Flag for jumping
    bool jumpCancelled; // Flag for jump cancel

    /* Double jumps */
    private int jumpCount = 0;
    public bool unlockedDoubleJump = false; // If false, only one jump allowed

    public bool unlockedSwordBeam = false;

    /* For checking if player is grounded */
    public LayerMask groundLayer;
    private bool isGrounded;
    public Transform feetPosition;
    public float groundCheckCircle;

    /* Dodging */
    public bool dodging = false;
    private float dodgeTimeRemaining = 0f;
    public float dodgeDuration = 0.125f;
    public float dodgeSpeed = 24f;
    public float dodgeRate = 1.5f;
    float nextDodgeTime = 0f;
    public bool isFacingRight = true;
    public bool isInvincible = false;

    /* Attack */
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 0.4f;
    public int attackDamage = 20;
    public float attackRate = 2f;
    public float nextAttackTime = 0f;

    [SerializeField] public TextMeshProUGUI attackDisplay;

    /* Sound Effects 
    public AudioSource takeDamage;
    public AudioSource sword;
    public AudioSource hurtEnemy; */

    public ProjectileLaunch projectileLaunch;

    Rigidbody2D rb; // Reference to player's rigidbody
    Collider2D coll; // Reference to player's collider object
    public SpriteRenderer sr; // Reference to player's sprite renderer
    public Animator animator;

    public Level level;

    public Camera mainCamera; // Reference to the main camera

    void Start() {
        rb = GetComponent<Rigidbody2D>(); // Get the player's rigidbody component
        coll = GetComponent<Collider2D>(); // Get the player's collider object
        sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        WalkHandler(); // Handle player walking
        JumpHandler(); // Handle player jumping

        attackDisplay.text = attackDamage.ToString();

        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y, -20f); // Adjust camera position
        mainCamera.transform.position = cameraPosition;

        if (Time.time >= nextDodgeTime) {
            if (Input.GetKeyDown(KeyCode.LeftShift)) { // If left shift key is pressed while moving horizontally
                dodging = true;
                dodgeTimeRemaining = dodgeDuration;
                StartCoroutine(FadeOutIn(dodgeDuration));
                nextDodgeTime = Time.time + 1f / dodgeRate;
                gameObject.layer = LayerMask.NameToLayer("PlayerDodge");
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlayerDodge"), LayerMask.NameToLayer("PlayersEnemies"), true);
            }
        }

        if (Time.time >= nextAttackTime) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                Attack();
                projectileLaunch.Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void WalkHandler() {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // Input on horizontal axis
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !isFacingRight) {
            Flip();
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && isFacingRight) {
            Flip();
        }

        if (dodging) {
            if (isFacingRight) { 
                rb.velocity = new Vector2(-dodgeSpeed, rb.velocity.y); // Apply dodge speed to player if dodging
            } else {
                rb.velocity = new Vector2(dodgeSpeed, rb.velocity.y);
            }
        } else {
            Vector2 velocity = new Vector2(horizontalInput * walkSpeed, rb.velocity.y); // Calculate player's velocity based on input and walk speed
            rb.velocity = velocity; // Apply velocity to player's rigidbody
        }

        if (dodging) { // Duration during dodge
            dodgeTimeRemaining -= Time.deltaTime;
            if (dodgeTimeRemaining <= 0) {
                dodging = false;
                gameObject.layer = LayerMask.NameToLayer("Player");
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlayerDodge"), LayerMask.NameToLayer("PlayersEnemies"), true);
            }
        }
    }

    void JumpHandler() { 
        if (Input.GetKeyDown(KeyCode.Space)) {
            isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircle, groundLayer);
            if (isGrounded) {
                jumpCount = 0;
            }
            if ((!unlockedDoubleJump && isGrounded) ^ (unlockedDoubleJump && (jumpCount < 2 || isGrounded))) { // Allow for up to 2 jumps if unlocked
                animator.SetBool("isJumping", true);
                float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumping = true;
                jumpCancelled = false;
                jumpTime = 0;
                jumpCount++;
            }
        }
        if (jumping) {
            jumpTime += Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Space)) {
                animator.SetBool("isJumping", false);
                jumpCancelled = true;
            }
            if (jumpTime > buttonTime) {
                animator.SetBool("isJumping", false);
                jumping = false;
            }
        }
    }

    private void FixedUpdate() {
        if (jumpCancelled && jumping && rb.velocity.y > 0) {
            rb.AddForce(Vector2.down * cancelRate);
        }
    }

    void Flip() {
        isFacingRight = !isFacingRight;

        Vector3 currentScale = transform.localScale;

        // Invert the x-axis scale to flip the object
        currentScale.x *= -1;

        // Update the object's scale with the flipped value
        transform.localScale = currentScale;
    }

    void Attack() {
        AudioManager.instance.sword.Play();
        StartCoroutine(DelayForDamage());
        animator.SetTrigger("Attack");
    }

    private IEnumerator DelayForDamage() {
        yield return new WaitForSeconds(0.25f);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies) {
            if (enemy.GetComponent<Skeleton>() != null) {
                enemy.GetComponent<Skeleton>().TakeDamage(attackDamage);
            }
            if (enemy.GetComponent<Zombie>() != null) {
                enemy.GetComponent<Zombie>().TakeDamage(attackDamage);
            }
            if (enemy.GetComponent<RangedSkeleton>() != null) {
                enemy.GetComponent<RangedSkeleton>().TakeDamage(attackDamage);
            }
            if (enemy.GetComponent<Ghost>() != null) {
                enemy.GetComponent<Ghost>().TakeDamage(attackDamage);
            }
            if (enemy.GetComponent<VoidReaper>() != null) {
                enemy.GetComponent<VoidReaper>().TakeDamage(attackDamage);
            }
        }
    }

    public void GainExp(int exp) {
        level.GainExp(exp);
    }

    void OnTriggerEnter2D(Collider2D collider) 
    { 
        if (collider.gameObject.tag == "DoubleJump") 
        { 
            unlockedDoubleJump = true;
            Destroy(collider.gameObject);  
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

    public void IncreaseAttack(int increasedAttack) {
        attackDamage += increasedAttack;
    }
}
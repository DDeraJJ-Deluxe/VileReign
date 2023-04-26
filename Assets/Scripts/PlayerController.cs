using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 8f;  // Player walk speed

    /* Variable jump height */
    public float buttonTime = 0.5f; // Limit of jump button pressed
    public float jumpHeight = 4.5f; // Max height of jump
    public float cancelRate = 100; // For jump-cancelling
    float jumpTime; // Store time during jump
    bool jumping; // Flag for jumping
    bool jumpCancelled; // Flag for jump cancel

    /* Double jumps */
    private int jumpCount = 0;
    public bool unlockedDoubleJump = false; // If false, only one jump allowed

    /* For checking if player is grounded */
    public LayerMask groundLayer;
    private bool isGrounded;
    public Transform feetPosition;
    public float groundCheckCircle;

    /* Dodging */
    private bool dodging = false;
    private float dodgeTimeRemaining = 0f;
    public float dodgeDuration = 0.25f;
    public float dodgeSpeed = 28f;

    Rigidbody2D rb; // Reference to player's rigidbody
    Collider2D coll; // Reference to player's collider object
    SpriteRenderer sr; // Reference to player's sprite renderer

    public Camera mainCamera; // Reference to the main camera

    void Start() {
        rb = GetComponent<Rigidbody2D>(); // Get the player's rigidbody component
        coll = GetComponent<Collider2D>(); // Get the player's collider object
        sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        WalkHandler(); // Handle player walking
        JumpHandler(); // Handle player jumping

        Vector3 cameraPosition = new Vector3(transform.position.x, 2.1f, -10f); // Adjust camera position
        mainCamera.transform.position = cameraPosition;

        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0) { // If left shift key is pressed while moving horizontally
            dodging = true;
            dodgeTimeRemaining = dodgeDuration;
            StartCoroutine(FadeOutIn(dodgeDuration));
        }
    }

    void WalkHandler() {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // Input on horizontal axis

        if (dodging) {
            rb.velocity = new Vector2(-horizontalInput * dodgeSpeed, rb.velocity.y); // Apply dodge speed to player if dodging
        } else {
            Vector2 velocity = new Vector2(horizontalInput * walkSpeed, rb.velocity.y); // Calculate player's velocity based on input and walk speed
            rb.velocity = velocity; // Apply velocity to player's rigidbody
        }

        if (dodging) { // Duration during dodge
            dodgeTimeRemaining -= Time.deltaTime;
            if (dodgeTimeRemaining <= 0) {
                dodging = false;
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
                jumpCancelled = true;
            }
            if (jumpTime > buttonTime) {
                jumping = false;
            }
        }
    }

    private void FixedUpdate() {
        if (jumpCancelled && jumping && rb.velocity.y > 0) {
            rb.AddForce(Vector2.down * cancelRate);
        }
    }

    void OnTriggerEnter2D(Collider2D collider) { 

    }

    /* Fades out and in for a given duration */
    IEnumerator FadeOutIn(float duration) {
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
    }
}
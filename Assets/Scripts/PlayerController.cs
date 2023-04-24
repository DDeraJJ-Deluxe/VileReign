using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 8f;  // Player walk speed

    /* Variable jumps */
    public float buttonTime = 0.5f; // Limit of jump button pressed
    public float jumpHeight = 4.5f; // Max height of jump
    public float cancelRate = 100; // For jump-cancelling
    float jumpTime; // Store time during jump
    bool jumping; // Flag for jumping
    bool jumpCancelled; // Flag for jump cancel

    /* Double jumps */
    private int jumpCount = 0;
    public bool unlockedDoubleJump = false; // If false, only one jump allowed

    public LayerMask groundLayer;
    private bool isGrounded;
    public Transform feetPosition;
    public float groundCheckCircle;

    Rigidbody2D rb; // Reference to player's rigidbody
    Collider2D coll; // Reference to player's collider object

    public Camera mainCamera; // Reference to the main camera

    void Start() {
        rb = GetComponent<Rigidbody2D>(); // Get the player's rigidbody component
        coll = GetComponent<Collider2D>(); // Get the player's collider object
    }

    void Update() {
        WalkHandler(); // Handle player walking
        JumpHandler(); // Handle player jumping

        Vector3 cameraPosition = new Vector3(transform.position.x, 2.1f, -10f); // Adjust camera position
        mainCamera.transform.position = cameraPosition;
    }

    void WalkHandler() {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // Input on horizontal axis

        Vector2 velocity = new Vector2(horizontalInput * walkSpeed, rb.velocity.y); // Calculate player's velocity based on input and walk speed

        rb.velocity = velocity; // Apply velocity to player's rigidbody
    }

    void JumpHandler() { 
        if (Input.GetKeyDown(KeyCode.Space)) {
            isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckCircle, groundLayer);
            if (isGrounded) {
                jumpCount = 0;
            }
            if ((!unlockedDoubleJump && isGrounded) ^ (unlockedDoubleJump && (jumpCount < 2 || isGrounded))) { // Allow for up to 2 jumps
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
}

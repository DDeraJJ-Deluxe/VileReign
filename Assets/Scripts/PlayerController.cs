using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float walkSpeed = 8f;  // Player walk speed
    public float buttonTime = 0.5f; // Limit of jump button pressed
    public float jumpHeight = 4.5f; // Max height of jump
    public float cancelRate = 100; // For jump-cancelling
    float jumpTime; // Store time during jump
    bool jumping; // Flag for jumping
    bool jumpCancelled; // Flag for jump cancel

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
        if (Input.GetKeyDown(KeyCode.Space) && CheckGrounded()) {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumping = true;
            jumpCancelled = false;
            jumpTime = 0;
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
        if(jumpCancelled && jumping && rb.velocity.y > 0) {
            rb.AddForce(Vector2.down * cancelRate);
        }
    }

    bool CheckGrounded() { 
        // Create a circle collider at the player's feet
        Vector2 boxSize = new Vector2(coll.bounds.size.x - 0.1f, 0.05f); // Set the size of the overlap box to be slightly smaller than the player's collider
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y - coll.bounds.extents.y); // Set the center of the overlap box to be just below the player's collider

        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0); // Check for overlapping colliders

        // Check if any of the overlapping colliders are considered as ground
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject != gameObject && (collider.gameObject.layer == LayerMask.NameToLayer("Ground") || collider.gameObject.layer == LayerMask.NameToLayer("Platform"))) {
                return true;
            }
        }

        return false;
    }

    void OnTriggerEnter2D(Collider2D collider) { 

    }
}

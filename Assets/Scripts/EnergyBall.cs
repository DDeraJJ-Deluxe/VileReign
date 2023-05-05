using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public Rigidbody2D projectileRb;
    public Transform playerTransform;
    public float speed;

    public float projectileLife;
    public float projectileCount;
    public int projectileDamage = 10;

    public Ghost ghost;
    public bool facingRight;

    // Start is called before the first frame update
    void Start() {
        projectileCount = projectileLife;
        ghost = GameObject.FindGameObjectWithTag("Ghost").GetComponent<Ghost>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        facingRight = ghost.isFacingRight;
        if (!facingRight) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        // Calculate the direction from the arrow to the player's position
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        // Set the arrow's velocity in that direction
        projectileRb.velocity = direction * speed;
    }

    // Update is called once per frame
    void Update() {
        projectileCount -= Time.deltaTime;
        if (projectileCount <= 0) {
            Destroy(gameObject);
        }
    }

/*
    private void FixedUpdate() {
        if (facingRight) {
            projectileRb.velocity = new Vector2(speed, projectileRb.velocity.y);
        } else {
            projectileRb.velocity = new Vector2(-speed, projectileRb.velocity.y);
        }
    }*/

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.GetComponent<PlayerHealth>() != null) {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(projectileDamage);
        } 
        Destroy(gameObject);
    }
}

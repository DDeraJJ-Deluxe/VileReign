using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D projectileRb;
    public Transform playerTransform;
    public float speed;

    public float projectileLife;
    public float projectileCount;
    public int projectileDamage = 10;

    public RangedSkeleton rangedSkeleton;
    public bool facingRight;

    // Start is called before the first frame update
    void Start() {
        projectileCount = projectileLife;
        rangedSkeleton = GameObject.FindGameObjectWithTag("Enemy").GetComponent<RangedSkeleton>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        facingRight = rangedSkeleton.isFacingRight;
        if (!facingRight) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        // Compute the direction from the arrow to the player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        if (playerTransform.position.y < transform.position.y) {
            direction += Vector2.up * 0.3f;
            direction = direction.normalized;
        } else {
            direction += Vector2.up * 0.35f;
            direction = direction.normalized;
        }
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
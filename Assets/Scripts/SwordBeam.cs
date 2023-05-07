using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBeam : MonoBehaviour
{
    public Rigidbody2D projectileRb;
    public float speed;

    public float projectileLife;
    public float projectileCount;
    public int projectileDamage = 20;

    public PlayerController playerController;
    public bool facingRight;

    // Start is called before the first frame update
    void Start() {
        projectileCount = projectileLife;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        facingRight = playerController.isFacingRight;
        if (!facingRight) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update() {
        projectileCount -= Time.deltaTime;
        if (projectileCount <= 0) {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if (facingRight) {
            projectileRb.velocity = new Vector2(speed, projectileRb.velocity.y);
        } else {
            projectileRb.velocity = new Vector2(-speed, projectileRb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.GetComponent<Skeleton>() != null) {
            collision.gameObject.GetComponent<Skeleton>().TakeDamage(projectileDamage);
        } 
        if (collision.gameObject.GetComponent<Zombie>() != null) {
            collision.gameObject.GetComponent<Zombie>().TakeDamage(projectileDamage);
        }
        if (collision.gameObject.GetComponent<RangedSkeleton>() != null) {
            collision.gameObject.GetComponent<RangedSkeleton>().TakeDamage(projectileDamage);
        }
        if (collision.gameObject.GetComponent<Ghost>() != null) {
            collision.gameObject.GetComponent<Ghost>().TakeDamage(projectileDamage);
        }
        if (collision.gameObject.GetComponent<VoidReaper>() != null) {
            collision.gameObject.GetComponent<VoidReaper>().TakeDamage(projectileDamage);
        } 
        Destroy(gameObject);
    }
}

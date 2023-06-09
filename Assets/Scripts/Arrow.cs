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

    public bool facingRight;
    Vector2 direction;

    // Start is called before the first frame update
    void Start() {
        projectileCount = projectileLife;
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Arrow"), LayerMask.NameToLayer("ArcherCollision"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Arrow"), LayerMask.NameToLayer("PlayersEnemies"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Arrow"), LayerMask.NameToLayer("Ghost"), true);

        if (playerTransform.position.x < transform.position.x) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        // Compute the direction from the arrow to the player
        direction = (playerTransform.position - transform.position).normalized;
        if (playerTransform.position.y < transform.position.y) {
            direction += Vector2.up * 0.3f;
            direction = direction.normalized;
        } else {
            direction += Vector2.up * 0.35f;
            direction = direction.normalized;
        }
        projectileRb.velocity = direction * speed;
    }

    void Update() {
        direction = projectileRb.velocity.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        projectileCount -= Time.deltaTime;
        if (projectileCount <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.GetComponent<PlayerHealth>() != null) {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(projectileDamage);
        } 
        Destroy(gameObject);
    }
}

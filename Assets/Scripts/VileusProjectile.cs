using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VileusProjectile : MonoBehaviour {

    public Rigidbody2D projectileRb;
    public Transform playerTransform;
    public float speed;

    public float projectileLife;
    public float projectileCount;
    public int projectileDamage = 20;

    public LordVileus lordVileus;
    public bool facingRight;

    // Start is called before the first frame update
    void Start() {
        projectileCount = projectileLife;
        lordVileus = GameObject.FindGameObjectWithTag("LordVileus").GetComponent<LordVileus>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("VileusProjectile"), LayerMask.NameToLayer("Platform"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("VileusProjectile"), LayerMask.NameToLayer("Ground"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("VileusProjectile"), LayerMask.NameToLayer("PlayersEnemies"), true);
        facingRight = lordVileus.isFacingRight;
        if (!facingRight) {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        projectileRb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Update is called once per frame
    void Update() {
        projectileCount -= Time.deltaTime;
        if (projectileCount <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<PlayerHealth>() != null) {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(projectileDamage);
        } 
        Destroy(gameObject);
    }
}

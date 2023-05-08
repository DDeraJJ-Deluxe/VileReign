using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidProjectile : MonoBehaviour {

    public Rigidbody2D projectileRb;

    public float speed;

    public float projectileLife;
    public float projectileCount;
    public int projectileDamage = 10;

    void Start() {
        projectileCount = projectileLife;
        projectileRb.velocity = new Vector2(0f, -speed);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("VoidProjectile"), LayerMask.NameToLayer("Platform"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("VoidProjectile"), LayerMask.NameToLayer("Ground"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("VoidProjectile"), LayerMask.NameToLayer("PlayersEnemies"), true);
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Block : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D body;

    // Health.
    [SerializeField] protected int maxHealth;
    [SerializeField, ReadOnly] protected int health;

    // Movement.
    [SerializeField] private float resistance = 0.985f;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    void FixedUpdate() {
        body.velocity *= resistance;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        ProcessCollision(collider);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Init() {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        body.isKinematic = true;
        gameObject.SetActive(true);
    }

    #endregion

    /* --- Collision --- */
    #region Collision

    private void ProcessCollision(Collider2D collider) {
        if (collider.GetComponent<Projectile>()) {
            body.isKinematic = false;
            health -= 1;
            CheckHealth();
        }
    }

    protected virtual void CheckHealth() {
        if (health <= 0) {
            Destroy(gameObject);
            return;
        }
        spriteRenderer.color = new Color(1f, (float)health / maxHealth, (float)health / maxHealth, 1f);
    }

    #endregion

}

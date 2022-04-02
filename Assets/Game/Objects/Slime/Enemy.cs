using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D body;

    // Health.
    [SerializeField] private int health;

    // Movement.
    [SerializeField] private float force = 150f;
    [SerializeField] private float resistance = 0.985f;
    [SerializeField] private float shootCooldown = 0.25f;
    [SerializeField] private float angleOffset = 7.5f;
    [SerializeField, ReadOnly] private float cooldown;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    void FixedUpdate() {
        transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.down, body.velocity));
        float deltaTime = Time.fixedDeltaTime;
        Cooldown(deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        ProcessCollision(collider);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        gameObject.SetActive(true);

        body.AddForce(Random.insideUnitCircle.normalized * force);
    }

    #endregion

    /* --- Movement --- */
    #region Movement

    private void Cooldown(float deltaTime) {
        cooldown -= deltaTime;
        if (cooldown <= 0f) {
            Shoot();
            cooldown = shootCooldown;
        }

        body.velocity *= resistance;
    }

    private void Shoot() {
        Vector2 direction = Quaternion.Euler(0f, 0f, Random.Range(-angleOffset, angleOffset)) * body.velocity.normalized;
        body.AddForce(direction * force);
    }

    #endregion

    /* --- Collision --- */
    #region Collision

    private void ProcessCollision(Collider2D collider) {
        if (collider.GetComponent<Projectile>()) {
            health -= 1;
            CheckHealth();
        }
    }

    private void CheckHealth() {
        if (health <= 0) {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    [HideInInspector] public SpriteRenderer spriteRenderer;

    // Health.
    [SerializeField] private int health;
    [SerializeField] private Sprite[] sprites;

    // Movement.
    [SerializeField] private Vector2 direction;
    [SerializeField] private Vector2 target;

    #endregion

    /* --- Unity --- */
    #region Unity

    void FixedUpdate() {
        Move();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        ProcessCollision(collider);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public void Init(Vector2 movementDirection) {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[(int)Mathf.Min(sprites.Length - 1, health)];

        target = transform.position;
        direction = movementDirection;

        gameObject.SetActive(true);
    }

    #endregion

    /* --- Movement --- */
    #region Movement

    private void Move() {
        if (target != (Vector2)transform.position) {
            Vector3 deltaPosition = ((Vector3)target - transform.position).normalized * Time.fixedDeltaTime * 5f;
            transform.position += deltaPosition;
            if ((target - (Vector2)transform.position).magnitude < GameRules.MovementPrecision) {
                transform.position = target;
            }
        }
    }

    public void MoveIn() {
        target += direction;
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
        spriteRenderer.sprite = sprites[(int)Mathf.Min(sprites.Length - 1, health - 1)];
    }

    #endregion

}

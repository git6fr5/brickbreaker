using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Hurt.
    public static float HurtBuffer = 0.5f;
    [Space(2), Header("Hurt")]
    [SerializeField, ReadOnly] public bool hurt = false;
    [SerializeField, ReadOnly] public float hurtTicks;
    // Knockback.
    [Space(2), Header("Knocked")]
    [SerializeField, ReadOnly] public bool knocked = false;
    [SerializeField, ReadOnly] private float knockDuration = 0f;

    // Components.
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D body;

    // Health.
    [SerializeField] protected int health;

    // Movement.
    [SerializeField] protected float force = 150f;
    [SerializeField] protected float resistance = 0.985f;
    [SerializeField] protected float shootCooldown = 0.25f;
    [SerializeField] protected float angleOffset = 7.5f;
    [SerializeField, ReadOnly] protected float cooldown;

    public AudioClip hitSound;
    public AudioClip hitWallSound;
    public AudioClip hurtSound;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    void FixedUpdate() {
        transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.down, body.velocity));
        float deltaTime = Time.fixedDeltaTime;
        if (hurt) {
            ProcessHurt(deltaTime);
        }
        if (knocked) {
            ProcessKnocked(deltaTime);
        }
        else {
            Cooldown(deltaTime);
        }

    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        ProcessCollision(collider);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    public virtual void Init() {
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

    private void ProcessHurt(float deltaTime) {
        hurtTicks += deltaTime;
        if (hurtTicks >= HurtBuffer) {
            hurt = false;
            hurtTicks = 0f;
        }
    }

    private void ProcessKnocked(float deltaTime) {
        knockDuration -= deltaTime;
        if (knockDuration <= 0) {
            knocked = false;
            knockDuration = 0f;
        }
    }

    public void Knock(Vector2 direction, float force, float duration) {
        knocked = true;
        knockDuration = duration;
        transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.down, direction));
        body.velocity = Vector2.down * force;
    }

    private void ProcessCollision(Collider2D collider) {
        if (hurt) {
            return;
        }

        if (collider.GetComponent<Projectile>() != null) {
            Screen.CameraShake(0.125f, HurtBuffer);
            
            health -= 1;
            hurt = true;

            // Rigidbody2D projectileBody = collider.GetComponent<Rigidbody2D>();
            // Knock(projectileBody.velocity.normalized, 15f, HurtBuffer / 4f);

            CheckHealth();
            Destroy(collider.gameObject);
        }
        else if (collider.GetComponent<WallBlock>() != null) {
            GameRules.PlaySound(hitWallSound, GetComponent<AudioSource>());
        }
        else {
            GameRules.PlaySound(hitSound, GetComponent<AudioSource>());
        }
    }

    private IEnumerator IEFlicker(float interval) {
        int flicks = (int)Mathf.Floor(HurtBuffer / interval);
        for (int i = 0; i < flicks; i++) {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            if (!hurt) { break; }
            yield return new WaitForSeconds(interval);
        }
        spriteRenderer.enabled = true;
        yield return null;
    }

    protected virtual void CheckHealth() {
        if (health <= 0) {
            GameRules.PlaySound(hurtSound);
            Destroy(gameObject);
            return;
        }
        StartCoroutine(IEFlicker(GameRules.FlickerRate));
    }

    #endregion

}

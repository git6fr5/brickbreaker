/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a character.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Controller : MonoBehaviour {

    /* --- Enumerations --- */
    #region Enumerations
    public enum Direction {
        Right, Left, Count
    }

    public enum Movement {
        Idle, Moving, Knocked, Count
    }

    #endregion

    /* --- Variables --- */
    #region Variables

    // Components.
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Rigidbody2D body;
    [HideInInspector] public BoxCollider2D hurtbox;

    // Health.
    [Space(2), Header("Health")]
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;

    // Movement.
    [Space(2), Header("Movement")]
    [SerializeField, ReadOnly] protected Vector2 movementInput;
    [SerializeField, ReadOnly] public Vector2 direction;
    [SerializeField, Range(1f, 20f)] protected float speed = 15f;
    [SerializeField, Range(1f, 20f)] protected float acceleration = 15f;
    [SerializeField, Range(0.9f, 0.995f)] protected float resistance = 0.985f;

    // Action.
    [Space(2), Header("Action")]
    [SerializeField] protected Weapon weapon;
    [SerializeField, ReadOnly] protected bool attackInput;
    
    // Hurt.
    [Space(2), Header("Hurt")]
    [SerializeField, ReadOnly] public bool hurt = false;
    [SerializeField, ReadOnly] public float hurtTicks;
    public static float HurtBuffer = 0.5f;

    // Knockback.
    [Space(2), Header("Knocked")]
    [SerializeField, ReadOnly] public bool knocked = false;
    [SerializeField, ReadOnly] private float knockDuration = 0f;

    // Flags.
    [Space(2), Header("Flags")]
    [SerializeField, ReadOnly] public Direction directionFlag;
    [SerializeField, ReadOnly] public Movement movementFlag;

    // Switches.
    [Space(2), Header("Switches")]
    [SerializeField, ReadOnly] public bool hover = false;
    [SerializeField, ReadOnly] public bool active = false;


    // Debug.
    [Space(2), Header("Debug")]
    [SerializeField, ReadOnly] protected float debugSpeed;
    [HideInInspector] protected Vector3 previousPosition;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Runs once before the first frame.
    void Start() {
        Init();
    }

    // Runs once every frame.
    void Update() {
        float deltaTime = Time.deltaTime;
        GetInput(deltaTime);
        GetFlags();
    }

    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        if (hurt) {
            ProcessHurt(deltaTime);
        }
        if (knocked) {
            ProcessKnocked(deltaTime);
        }
        else {
            ProcessTarget(deltaTime);
            ProcessMovement(deltaTime);
            ProcessAttack(deltaTime);
        }
        Debug(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    // Initializes this script.
    public virtual void Init() {
        // Caching components.
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Vector2.down;
        health = maxHealth;
        ResetBody(body);
    }

    private static void ResetBody(Rigidbody2D rigidbody) {
        rigidbody.gravityScale = 0f;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    #endregion

    /* --- Processes --- */
    #region Processing

    // Gets the input for this frame.
    protected virtual void GetInput(float deltaTime) {
    }

    private void ProcessTarget(float deltaTime) {
        print("Setting Direction");
        direction = (GameRules.MousePosition - (Vector2)transform.position).normalized;
    }

    // Processes the movemment inputs.
    private void ProcessMovement(float deltaTime) {
        // Process the physics.
        Vector2 targetVelocity = (Vector3)movementInput.normalized * speed;
        if (attackInput) { targetVelocity *= 0.25f; }
        Vector2 deltaVelocity = (targetVelocity - body.velocity) * acceleration * deltaTime;
        body.velocity += deltaVelocity;
        // Resistance
        if (targetVelocity == Vector2.zero) {
            body.velocity *= resistance;
        }
        // Check for released inputs.
        if (targetVelocity.y == 0f && Mathf.Abs(body.velocity.y) < GameRules.MovementPrecision) {
            body.velocity = new Vector2(body.velocity.x, 0f);
        }
        if (targetVelocity.x == 0f && Mathf.Abs(body.velocity.x) < GameRules.MovementPrecision) {
            body.velocity = new Vector2(0f, body.velocity.y);
        }
    }

    private void ProcessKnocked(float deltaTime) {
        knockDuration -= deltaTime;
        if (knockDuration <= 0) {
            knocked = false;
            knockDuration = 0f;
        }
    }

    private void ProcessHurt(float deltaTime) {
        hurtTicks += deltaTime;
        if (hurtTicks >= HurtBuffer) {
            hurt = false;
            hurtTicks = 0f;
        }
    }

    private void ProcessAttack(float deltaTime) {
        if (attackInput && weapon != null) {
            weapon.Activate(direction);
        }
    }

    public void Knock(Vector2 direction, float force, float duration) {
        knocked = true;
        knockDuration = duration;
        body.velocity = direction * force;
    }

    public void Hurt(float damage) {
        if (!hurt) {
            health -= damage;
            hurt = true;
            hurtTicks = 0f;
        }
    }

    #endregion

    /* --- Flags --- */
    private void GetFlags() {
        movementFlag = MovementFlag();
        directionFlag = DirectionFlag();
    }

    protected Direction DirectionFlag() {
        if (direction.x > 0f) {
            return Direction.Right;
        }
        else if (direction.x < 0f) {
            return Direction.Left;
        }
        return directionFlag;
    }

    protected Movement MovementFlag() {
        if (knocked) {
            return Movement.Knocked;
        }
        else if (movementInput != Vector2.zero) {
            return Movement.Moving;
        }
        else {
            return Movement.Idle;
        }
    }

    /* --- Debug --- */
    #region Debug

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction);
    }

    private void Debug(float deltaTime) {
        debugSpeed = (transform.position - previousPosition).magnitude / deltaTime;
        previousPosition = transform.position;
    }

    #endregion

}

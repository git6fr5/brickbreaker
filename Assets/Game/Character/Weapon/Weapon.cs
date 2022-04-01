/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Weapon : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    [HideInInspector] protected SpriteRenderer spriteRenderer;

    // User.
    [SerializeField] private Controller user;
    [SerializeField, ReadOnly] public Vector2 targetDirection;

    // Damage
    [SerializeField] private float damage;

    // Momemtum.
    [SerializeField] private float fireSpeed;
    [SerializeField] private float knockbackSpeed;
    [SerializeField] private float fireCooldown;
    [SerializeField, ReadOnly] private float cooldown;

    // Pivots.
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform hand;

    // Projectile.
    [SerializeField] private Projectile projectile;

    // Callbacks.
    public Vector3 TipPosition => GetTipPosition();

    #endregion

    /* --- Uniry --- */
    #region Unity

    void Start() {
        Init();
    }

    void Update() {
        Render();

    }

    void FixedUpdate() {
        Cooldown();
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Rendering

    private Vector3 GetTipPosition() {
        return firePoint.transform.position;
    }

    void Render() {
        int order = (int)user.directionFlag == 1 ? 1 : -1;
        float angle = Vector2.SignedAngle(Vector2.up, user.direction);
        spriteRenderer.sortingOrder = user.spriteRenderer.sortingOrder - order;
        hand.eulerAngles = new Vector3(0f, 0f, angle);
    }

    #endregion

    #region Attack

    public void Activate(Vector2 direction) {
        if (cooldown > 0f) {
            return;
        }

        // user.Knock(-direction, knockbackSpeed, fireCooldown);
        projectile.Fire(firePoint.position, direction, fireSpeed);
        cooldown = fireCooldown;
    }

    private void Cooldown() {
        cooldown -= Time.fixedDeltaTime;
        if (cooldown < 0f) {
            cooldown = 0f;
        }
    }

    #endregion

    /* --- Debug --- */
    #region Debug

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
    }

    #endregion


}

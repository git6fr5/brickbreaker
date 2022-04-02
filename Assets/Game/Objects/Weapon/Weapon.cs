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

    // Damage
    [SerializeField] private float damage;

    // Momemtum.
    [SerializeField] private float fireSpeed;
    [SerializeField] private float fireCooldown;
    [SerializeField, ReadOnly] private float cooldown;

    // Pivots.
    [SerializeField] private Transform firePoint;

    // Projectile.
    [SerializeField] public Projectile projectile;

    #endregion

    /* --- Uniry --- */
    #region Unity

    void Start() {
        Init();
    }

    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Cooldown(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Attack

    public void Activate(Vector2 direction) {
        if (cooldown > 0f) {
            return;
        }
        projectile.Fire(firePoint.position, direction, fireSpeed);
        cooldown = fireCooldown;
    }

    private void Cooldown(float deltaTime) {
        cooldown -= deltaTime;
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

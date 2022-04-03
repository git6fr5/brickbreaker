/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    /* --- Variables --- */
    #region Variables.

    // Components.
    public Rigidbody2D body;

    // Settings.
    public float lifeTime = 3f;
    public float flatSpeed = 3f;
    public float resistance = 0.985f;

    public AudioClip hitSound;

    #endregion

    /* --- Unity --- */
    #region Unity

    void FixedUpdate() {
        transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.down, body.velocity));
        if (body.velocity.sqrMagnitude > flatSpeed * flatSpeed) {
            body.velocity *= resistance;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        GameRules.PlaySound(hitSound, GetComponent<AudioSource>());
    }

    #endregion

    /* --- Initalization --- */
    #region Initialization

    public void Fire(Vector2 origin, Vector2 direction, float speed) {
        Projectile projectile = Instantiate(gameObject, origin, Quaternion.identity, null).GetComponent<Projectile>();
        projectile.Init(direction, speed);
        Destroy(projectile.gameObject, lifeTime);
    }
   
    void Init(Vector2 direction, float speed) {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        gameObject.SetActive(true);
        body.velocity = direction * speed;
    }

    #endregion

}

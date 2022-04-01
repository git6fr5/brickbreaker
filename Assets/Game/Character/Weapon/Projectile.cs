/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

    /* --- Variables --- */
    #region Variables.

    // Components.
    public Rigidbody2D body;

    // Settings.
    public float lifeTime = 3f;

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

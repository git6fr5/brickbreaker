/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : Controller {

    /* --- Input --- */
    #region Input

    protected override void GetInput(float deltaTime) {
        base.GetInput(deltaTime);
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        attackInput = true; // Input.GetMouseButtonDown(0) ? true : (Input.GetMouseButtonUp(0) ? false : attackInput);
    }

    #endregion

}

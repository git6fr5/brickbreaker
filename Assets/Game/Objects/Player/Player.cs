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

    public int Health => health;
    public int MaxHealth => maxHealth;

    /* --- Input --- */
    #region Input

    protected override void GetInput(float deltaTime) {
        base.GetInput(deltaTime);
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        attackInput = true; // Input.GetMouseButtonDown(0) ? true : (Input.GetMouseButtonUp(0) ? false : attackInput);
    }

    public void AddHeart() {
        health += 1;
        UpdateColor();

    }

    protected override void CheckHealth() {
        base.CheckHealth();
        UpdateColor();
    }

    private void UpdateColor() {
        float intensity = Mathf.Pow((float)health / maxHealth, 2);
        float intensity2 = Mathf.Pow((float)health / maxHealth, 4);

        spriteRenderer.color = new Color(1f, intensity, intensity2, 1f);
        weapon.GetComponent<SpriteRenderer>().color = new Color(1f, intensity, intensity2, 1f);
        weapon.projectile.GetComponent<SpriteRenderer>().color = new Color(1f, intensity, intensity2, 1f);
    }

    #endregion

}

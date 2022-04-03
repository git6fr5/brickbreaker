using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingEnemy : Enemy {

    public float scale;
    public float slowFactor;

    public AudioClip eatSound;

    public override void Init() {
        base.Init();
        transform.localScale = new Vector3(1f, 1f, 1f) * scale / 1.5f / (float)health;
    }

    protected override void CheckHealth() {
        base.CheckHealth();
        if (health > 0) {
            GameRules.PlaySound(eatSound);
            transform.localScale = new Vector3(1f, 1f, 1f) * scale / 1.5f / (float)health;
            force = force * slowFactor;
        }
    }

}

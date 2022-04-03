using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingEnemy : Enemy {

    [SerializeField, ReadOnly] public int depth = 1;
    public int maxDepth;

    public float forceMultiplier;

    public override void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        gameObject.SetActive(true);

        body.AddForce(Random.insideUnitCircle.normalized * force);
        transform.localScale = new Vector3(1f, 1f, 1f) * 2f / ((float)depth * 0.75f);
    }

    protected override void CheckHealth() {
        if (health <= 0) {
            if (depth < maxDepth) {
                Split();
                Split();
            }
            GameRules.PlaySound(hurtSound);
            Destroy(gameObject);
            return;
        }

    }

    private void Split() {
        SplittingEnemy newSplit = Instantiate(gameObject, transform.position, Quaternion.identity, null).GetComponent<SplittingEnemy>();
        newSplit.health = 1;
        newSplit.depth = depth + 1;
        newSplit.force = force * forceMultiplier;
    }
}

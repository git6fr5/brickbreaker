using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBlock : Block {

    protected override void CheckHealth() {
        if (health <= 0) {
            Player[] players = (Player[])GameObject.FindObjectsOfType(typeof(Player));
            for (int i = 0; i < players.Length; i++) {
                if (players[i] != null && players[i].gameObject.activeSelf) {
                    players[i].AddHeart();
                }
            }
            Destroy(gameObject);
            return;
        }
        spriteRenderer.color = new Color(1f, (float)health / maxHealth, (float)health / maxHealth, 1f);
    }

}

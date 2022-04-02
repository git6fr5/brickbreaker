using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    
    List<GameObject> hearts = new List<GameObject>();
    public GameObject heart;

    Player player;

    void Awake() {
        player = transform.parent.GetComponent<Player>();
        transform.parent = null;
        transform.position = new Vector3(18f, -17.75f, 0f);
    }

    // Update is called once per frame
    void Update() {
        if (hearts.Count == player.Health) {
            return;
        }

        if (hearts != null) {
            for (int i = 0; i < hearts.Count; i++) {
                if (hearts[i] != null) {
                    Destroy(hearts[i]);
                }
            }
        }
        hearts = new List<GameObject>();

        for (int i = 0; i < player.Health; i++) {
            Vector3 position = new Vector3((float)(1.5f * i) - player.Health / 2f, 0f, 0f);
            GameObject heartObject = Instantiate(heart, Vector3.zero, Quaternion.identity, transform);
            heartObject.transform.localPosition = position;
            heartObject.SetActive(true);
            hearts.Add(heartObject);
        }
    }
}

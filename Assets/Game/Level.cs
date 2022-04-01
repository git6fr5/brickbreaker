using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public List<Block> blocks = new List<Block>();
    public Block block;

    void Start() {
        StartCoroutine(IECloseIn());
    }

    void Update() {
        print(Screen.ScreenSize);
    }

    private IEnumerator IECloseIn() {

        while (true) {

            yield return new WaitForSeconds(3f);

            List<Block> temp = new List<Block>();
            for (int i = 0; i < blocks.Count; i++) {
                if (blocks[i] != null) {
                    temp.Add(blocks[i]);
                }
            }
            blocks = temp;

            for (int i = 0; i < Screen.ScreenSize.x; i++) {
                Vector2 position = new Vector2(i - Screen.ScreenSize.x / 2f + 0.5f, Screen.ScreenSize.y / 2f + 1f);
                Block newBlock = Instantiate(block.gameObject, position, Quaternion.identity, transform).GetComponent<Block>();
                newBlock.Init(new Vector2(0f, -1f));
                blocks.Add(newBlock);
            }

            for (int i = 0; i < blocks.Count; i++) {
                blocks[i].MoveIn();
            }
        }

    }

}

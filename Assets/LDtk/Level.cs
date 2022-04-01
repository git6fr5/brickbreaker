using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour {

    public Environment environment;

    public int height;
    public int width;
    public int gridSize;

    public List<Entity> entities;

    public Vector3 GridToWorldPosition(Vector2Int gridPosition) {
        return new Vector3(gridPosition.x + 0.5f, - gridPosition.y + 0.5f, 0f) + transform.position;
    }

    public Vector3Int GridToTilePosition(Vector2Int gridPosition) {
        return new Vector3Int(gridPosition.x, -gridPosition.y, 0);
    }

}

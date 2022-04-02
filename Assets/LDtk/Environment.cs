using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

/// <summary>
/// Stores specific data on how to generate the level.
/// </summary>
public class Environment : MonoBehaviour {

    /* --- Components --- */
    // Entities.
    [SerializeField] public Transform entityParentTransform; // The location to look for the entities.

    /* --- Properties --- */
    [SerializeField, ReadOnly] public List<Entity> entities; // The set of entities found from the parent transform.
    
    // Tiles.
    [SerializeField] public RuleTile ground;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        RefreshEntities();
    }

    /* --- Entity Methods --- */
    // Refreshes the set of entities.
    void RefreshEntities() {
        entities = new List<Entity>();
        foreach (Transform child in entityParentTransform) {
            FindAllEntitiesInTransform(child, ref entities);
        }
    }

    // Recursively searches through the transform for all entity components.
    void FindAllEntitiesInTransform(Transform parent, ref List<Entity> entityList) {

        // If we've found an entity, don't go any deeper.
        if (parent.GetComponent<Entity>() != null) {
            entityList.Add(parent.GetComponent<Entity>());
        }
        else if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                FindAllEntitiesInTransform(child, ref entityList);
            }
        }
    }

    // Returns the first found entity with a matching ID.
    public Entity GetEntityByVectorID(Vector2Int vectorID, List<Entity> entityList) {
        for (int i = 0; i < entityList.Count; i++) {
            if (entityList[i].vectorID == vectorID) {
                return entityList[i];
            }
        }
        return null;
    }

}

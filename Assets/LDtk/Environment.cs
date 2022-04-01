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

    [SerializeField] public AudioSource aliveMusic;
    public static AudioSource AliveMusic;

    [SerializeField] public AudioSource deadMusic;
    public static AudioSource DeadMusic;


    [SerializeField] public AudioSource ambience;

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        RefreshEntities();

        AliveMusic = aliveMusic;
        DeadMusic = deadMusic;

    }

    public AnimationCurve musicCurve;
    public AnimationCurve panCurve;

    void Update() {

        if (GameRules.MainPlayer != null && !GameRules.MainPlayer.Alive) {
            AliveMusic.volume = 0.2f * musicCurve.Evaluate(GameRules.MainPlayer.RespawnRatio);
            DeadMusic.volume = 0.25f - 0.25f * musicCurve.Evaluate(GameRules.MainPlayer.RespawnRatio);
            // DeadMusic.panStereo = 0.5f * panCurve.Evaluate(GameRules.MainPlayer.RespawnRatio);
        }
        else {
            AliveMusic.volume = 0.2f;
        }

    }

    public static void SwitchMusic(bool alive) {

        DeadMusic.gameObject.SetActive(!alive);

        if (alive) {
            // AliveMusic.timeSamples = DeadMusic.timeSamples;
        }
        else {
            DeadMusic.timeSamples = 0; // AliveMusic.timeSamples;
            AliveMusic.timeSamples = 0;
        }

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

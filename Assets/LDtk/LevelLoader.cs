/* --- Libraries --- */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using LDtkUnity;

/* --- Definitions --- */
using LDtkLevel = LDtkUnity.Level;

/// <summary>
/// Loads a level from the lDtk Data into the level using the environment.
/// </summary>
public class LevelLoader : MonoBehaviour {

    /* --- Static Variables --- */
    // Layer Names
    public static string EntityLayer = "Entities";
    public static string LayoutLayer = "Layout";
    public static string LevelTag = "World_Level_";

    /* --- Data Structures --- */
    public class LDtkTileData {

        /* --- Properties --- */
        public Vector2Int vectorID;
        public Vector2Int gridPosition;
        public int index;

        /* --- Constructor --- */
        public LDtkTileData(Vector2Int vectorID, Vector2Int gridPosition, int index = 0) {
            this.vectorID = vectorID;
            this.gridPosition = gridPosition;
            this.index = index;
        }

    }

    /* --- Components --- */
    [SerializeField] public LDtkComponentProject lDtkData;
    [SerializeField] public Level level;

    /* --- Parameters --- */
    [SerializeField] public bool load;
    [SerializeField] public int id;


    /* --- Unity --- */
    // Runs once before the first frame.
    private void Update() {
        if (load) {
            Load();
            load = false;
        }
    }

    public void Load() {
        ResetLevel(level);
        OpenLevel(id);
    }

    /* --- Methods --- */
    private void OpenLevel(int id) {
        LDtkLevel ldtkLevel = GetLevelByID(lDtkData, id);
        OpenLevel(ldtkLevel);
    }

    public LDtkLevel GetLevelByID(LDtkComponentProject lDtkData, int id) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();
        print(json);

        // Read the json data.
        level.gridSize = (int)json.DefaultGridSize;
        level.height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        level.width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

        LDtkUnity.Level[] levels = json.Worlds[0].Levels;
        for (int i = 0; i < levels.Length; i++) {
            if (levels[i].Identifier == LevelTag + id.ToString()) {
                return levels[i];
            }
        }

        Debug.Log("Could not find room");
        return null;
    }

    protected void OpenLevel(LDtkLevel ldtkLevel) {

        if (ldtkLevel != null) {
            // Load the entity data.
            int gridSize = level.gridSize;

            List<LDtkTileData> entityData = LoadLayer(ldtkLevel, EntityLayer, gridSize);
            List<Entity> entityList = level.environment.entities;
            level.entities = LoadEntities(entityData, entityList);

        }

    }

    private void ResetLevel(Level level) {

        if (level.entities != null) {
            print("Resetting Entities");
            for (int i = 0; i < level.entities.Count; i++) {
                if ( level.entities[i] != null) {
                    Destroy(level.entities[i].gameObject);
                }
            }
        }
        level.entities = new List<Entity>();

    }

    private LDtkUnity.LayerInstance GetLayer(LDtkUnity.Level ldtkLevel, string layerName) {
        // Itterate through the layers in the level until we find the layer.
        for (int i = 0; i < ldtkLevel.LayerInstances.Length; i++) {
            LDtkUnity.LayerInstance layer = ldtkLevel.LayerInstances[i];
            if (layer.IsTilesLayer && layer.Identifier == layerName) {
                return layer;
            }
        }
        return null;
    }

    // Returns the vector ID's of all the tiles in the layer.
    private List<LDtkTileData> LoadLayer(LDtkUnity.Level ldtkLevel, string layerName, int gridSize, List<LDtkTileData> layerData = null) {

        if (layerData == null) { layerData = new List<LDtkTileData>(); }

        LDtkUnity.LayerInstance layer = GetLayer(ldtkLevel, layerName);
        if (layer != null) {
            // Itterate through the tiles in the layer and get the directions at each position.
            for (int index = 0; index < layer.GridTiles.Length; index++) {

                // Get the tile at this index.
                LDtkUnity.TileInstance tile = layer.GridTiles[index];

                // Get the position that this tile is at.
                Vector2Int gridPosition = tile.UnityPx / gridSize;
                Vector2Int vectorID = new Vector2Int((int)(tile.Src[0]), (int)(tile.Src[1])) / gridSize;

                // Construct the data piece.
                LDtkTileData tileData = new LDtkTileData(vectorID, gridPosition, index);
                layerData.Add(tileData);
            }

        }
        return layerData;
    }

    private List<Entity> LoadEntities(List<LDtkTileData> entityData, List<Entity> entityList) {

        List<Entity> entities = new List<Entity>();

        for (int i = 0; i < entityData.Count; i++) {
            // Get the entity based on the environment.
            Entity entityBase = level.environment.GetEntityByVectorID(entityData[i].vectorID, entityList);
            if (entityBase != null) {

                // Instantiate the entity
                Vector3 entityPosition = level.GridToWorldPosition(entityData[i].gridPosition);
                Entity newEntity = Instantiate(entityBase.gameObject, entityPosition, Quaternion.identity, level.transform).GetComponent<Entity>();
                newEntity.transform.localPosition = entityPosition;
                newEntity.transform.localRotation = entityBase.transform.localRotation;

                // Set up the entity.
                newEntity.gameObject.SetActive(true);
                newEntity.gridPosition = entityData[i].gridPosition;
                newEntity.gameObject.SetActive(true);

                // Add the entity to the list
                entities.Add(newEntity);
            }
        }
        return entities;
    }

}

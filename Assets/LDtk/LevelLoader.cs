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
    public static string GhostLayer = "Ghost";

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

    public AudioSource audioSource;
    public void Load() {
        audioSource.Play();
        ResetLevel(level);
        OpenLevel(id);
        GameRules.Init();
    }

    /* --- Methods --- */
    private void OpenLevel(int id) {
        LDtkLevel ldtkLevel = GetLevelByID(lDtkData, id);
        OpenLevel(ldtkLevel);
    }

    public LDtkLevel GetLevelByID(LDtkComponentProject lDtkData, int id) {

        // Get the json file from the LDtk Data.
        LdtkJson json = lDtkData.FromJson();

        // Read the json data.
        level.gridSize = (int)json.DefaultGridSize;
        level.height = (int)(json.DefaultLevelHeight / json.DefaultGridSize);
        level.width = (int)(json.DefaultLevelWidth / json.DefaultGridSize);

        int intId = 0;
        for (int i = 0; i < json.Levels.Length; i++) {
            if (json.Levels[i].Identifier == "Level_" + id.ToString()) {
                return json.Levels[i];
            }
        }

        
        //// Grab the level by the id.
        //if (id < json.Levels.Length && id >= 0) {
        //    return json.Levels[id];
        //}
        Debug.Log("Could not find room");
        return null;
    }

    protected void OpenLevel(LDtkLevel ldtkLevel) {

        if (ldtkLevel != null) {
            // Load the entity data.
            int gridSize = level.gridSize;
            List<LDtkTileData> entityData = LoadLayer(ldtkLevel, GhostLayer, gridSize);
            List<Entity> entityList = level.environment.entities;
            level.entities = LoadEntities(entityData, entityList);
            Order(ref level.entities, ldtkLevel);
        }

    }

    private void ResetLevel(Level level) {

        if (GameRules.MainPlayer != null) {
            Destroy(GameRules.MainPlayer.corpse.gameObject);
        }

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

    private void Order(ref List<Entity> entities, LDtkLevel ldtkLevel) {

        List<LDtkTileData> orderData = LoadLayer(ldtkLevel, "Controls", 16);

        Pole[] poles = new Pole[6];

        for (int i = 0; i < entities.Count; i++) {
            if (entities[i].GetComponent<Pole>() != null) {

                Pole pole = entities[i].GetComponent<Pole>();
                int id = -1;
                for (int j = 0; j < orderData.Count; j++) {
                    if (orderData[j].gridPosition == entities[i].gridPosition) {
                        id = orderData[j].vectorID.x;
                    }
                }

                if (id != -1 && id < 6) {
                    poles[id] = pole;
                }

            }
        }

        for (int i = 0; i < poles.Length; i+=2) {
            if (poles[i] != null && poles[i + 1] != null) {
                poles[i].Connect(poles[i + 1]);
                poles[i + 1].Connect(poles[i]);
            }
        }


    }

}

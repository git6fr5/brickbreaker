/* --- Unity --- */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameRules : MonoBehaviour {


    /* --- Static Variables --- */
    #region Static Variables

    // Tags.
    public static string PlayerTag = "Player";

    // Instance.
    public static GameRules Instance;
    // Player.
    public static Player MainPlayer;
    // Camera.
    public static UnityEngine.Camera MainCamera;
    public static Vector2 MousePosition => (Vector2)MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
    // Movement.
    public static float VelocityDamping = 0.95f;
    public static float MovementPrecision = 0.05f;
    public static float GravityScale = 0f;
    // Animation.
    public static float FrameRate = 24f;
    public static float FlickerRate = 0.075f;

    // Ticks.
    public static float Ticks;

    #endregion

    /* --- Variables --- */
    #region Variables

    [Space(2), Header("General")]
    [SerializeField] private Player mainPlayer;
    [SerializeField] private float movementPrecision = 0.05f;
    [SerializeField] private float frameRate = 24f;
    [SerializeField] private float timeScale = 1f;
    
    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        Init();
    }

    private void Update() {
        SetRules();
    }

    private void FixedUpdate() {
        Ticks += Time.fixedDeltaTime;
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        SetRules();
    }

    private void SetRules() {
        // Set these static variables.
        MainCamera = Camera.main;
        MainPlayer = mainPlayer;
        MovementPrecision = movementPrecision;
        FrameRate = frameRate;
        Time.timeScale = timeScale;

        // Instance
        Instance = this;
    }

    #endregion

    /* --- Generics --- */
    #region Generics

    public static List<T> GetAllWithinRadius<T>(Vector3 origin, float radius) {
        List<T> list = new List<T>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, radius);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].GetComponent<T>() != null) {
                list.Add(colliders[i].GetComponent<T>());
            }
        }
        return list;
    }

    #endregion

}


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
    public static Player MainPlayer => Instance.mainPlayer;
    public static LevelLoader MainLoader => Instance.mainLoader;
    // Camera.
    public static UnityEngine.Camera MainCamera => Camera.main;
    public static Vector2 MousePosition => (Vector2)MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
    // Movement.
    public static float VelocityDamping = 0.95f;
    public static float MovementPrecision => Instance.movementPrecision;
    public static float GravityScale = 0f;
    // Animation.
    public static float FlickerRate = 0.075f;

    // Ticks.
    public static float Ticks;

    #endregion

    /* --- Variables --- */
    #region Variables

    [Space(2), Header("General")]
    [SerializeField] private Player mainPlayer;
    [SerializeField] private LevelLoader mainLoader;

    [SerializeField] private float movementPrecision = 0.05f;
    [SerializeField] private float timeScale = 1f;
    
    private bool paused;
    public static bool Paused => Instance.paused;

    private float pausecharge = 0f;
    public static float PauseCharge => Instance.pausecharge;

    public static bool Dead => MainPlayer == null;
    private float deathCharge;
    public static float DeathCharge => Instance.deathCharge;

    private bool win;
    private float winCharge;
    public static float WinCharge => Instance.winCharge;

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
        float deltaTime = Time.fixedDeltaTime;
        Ticks += deltaTime;
        print(Time.fixedDeltaTime);
        CheckWin(deltaTime);
        if (win) { return; }
        CheckLose(deltaTime);
    }

    private void CheckWin(float deltaTime) {
        Enemy[] enemies = (Enemy[])GameObject.FindObjectsOfType(typeof(Enemy));
        win = enemies == null || enemies.Length == 0;
        if (win) {
            if (winCharge == 0f) {
                Screen.CameraShake(0.25f, 1f);
            }
            winCharge += Time.fixedDeltaTime;
        }
        else {
            winCharge = 0f;
        }

        if (winCharge > 1.25f) {
            Win();
            winCharge = 0f;
        }
    }

    private void CheckLose(float deltaTime) {
        if (Dead) {
            if (deathCharge == 0f) {
                Screen.CameraShake(0.25f, 1f);
            }
            deathCharge += Time.fixedDeltaTime;
        }
        else {
            deathCharge = 0f;
        }

        if (deathCharge > 1.25f) {
            Lose();
            deathCharge = 0f;
        }
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        SetRules();
    }

    private void SetRules() {
        // Set these static variables.
        Player[] players = (Player[])GameObject.FindObjectsOfType(typeof(Player));
        if (players == null || players.Length == 0) {
            mainPlayer = null;
        }
        else {
            mainPlayer = players[0];
        }
        Time.timeScale = timeScale;

        // Instance
        Instance = this;

        if (paused && Input.GetMouseButton(0)) {
            pausecharge += 0.05f;
        }
        else {
            pausecharge = 0f;
        }

        if (pausecharge > 1f) {
            Screen.CameraShake(0.25f, 1f);
            timeScale = 1f;
            paused = false;
        }

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

    public void Lose() {
        if (MainLoader != null) {
            Projectile[] projectiles = (Projectile[])GameObject.FindObjectsOfType(typeof(Projectile));
            for (int i = 0; i < projectiles.Length; i++) {
                Destroy(projectiles[i].gameObject);
            }
            MainLoader.Load();
            Pause(3f);
        }
    }

    public void Win() {
        if (MainLoader != null) {
            Projectile[] projectiles = (Projectile[])GameObject.FindObjectsOfType(typeof(Projectile));
            for (int i = 0; i < projectiles.Length; i++) {
                Destroy(projectiles[i].gameObject);
            }

            if (MainLoader.GetLevelByID(MainLoader.lDtkData, MainLoader.id + 1) != null) {
                MainLoader.id += 1;
                MainLoader.Load();
                Pause(3f);
            }
            else {
                SceneManager.LoadScene("End");
            }
        }
    }

    public void Pause(float duration) {
        timeScale = 0f;
        paused = true;
    }

    #endregion

}


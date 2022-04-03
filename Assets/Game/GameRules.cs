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

    public bool dead;
    public static bool Dead => Instance.dead;
    private float deathCharge;
    public static float DeathCharge => Instance.deathCharge;

    private bool win;
    private float winCharge;
    public static float WinCharge => Instance.winCharge;

    public AudioClip loseSound;
    public AudioClip winSound;

    #endregion

    /* --- Unity --- */
    #region Unity

    private void Update() {
        if (Instance == null) {
            Init();
            return;
        }
        CheckPlayer();
        CheckPause();
        Time.timeScale = timeScale;
    }

    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        Ticks += deltaTime;
        CheckWin(deltaTime);
        CheckLose(deltaTime);
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    private void Init() {
        Instance = this;
        mainLoader.Load();
        Pause();
    }

    #endregion

    /* --- Generics --- */
    #region Generics

    private void CheckPlayer() {
        Player[] players = (Player[])GameObject.FindObjectsOfType(typeof(Player));
        if (players == null || players.Length == 0) {
            mainPlayer = null;
        }
        else {
            mainPlayer = players[0];
        }
    }

    private void CheckPause() {
        pausecharge = paused && Input.GetMouseButton(0) ? pausecharge + 0.05f : 0f;

        if (pausecharge > 1f) {
            Screen.CameraShake(0.25f, 1f);
            timeScale = 1f;
            paused = false;
        }
    }

    private void CheckWin(float deltaTime) {
        Enemy[] enemies = (Enemy[])GameObject.FindObjectsOfType(typeof(Enemy));
        win = enemies == null || enemies.Length == 0;

        if (win && winCharge == 0f) {
            PlaySound(winSound);
            Screen.CameraShake(0.25f, 1f); 
        }
        winCharge = win ? winCharge + deltaTime / 1.75f : 0f;

        if (winCharge > 1.05f) {
            Win();
            winCharge = 0f;
        }
    }

    private void CheckLose(float deltaTime) {
        if (win) { return; }
        dead = MainPlayer == null;

        if (dead && deathCharge == 0f) {
            PlaySound(loseSound);
            Screen.CameraShake(0.25f, 1f); 
        }
        deathCharge = dead ? deathCharge + deltaTime / 1.75f : 0f;

        if (deathCharge > 1.05f) {
            Lose();
            deathCharge = 0f;
        }
    }

    public void Lose() {
        ClearProjectiles();
        mainLoader.Load();
        Pause();
    }

    public void Win() {
        ClearProjectiles();
        if (mainLoader.GetLevelByID(MainLoader.lDtkData, MainLoader.id + 1) != null) {
            mainLoader.id += 1;
            mainLoader.Load();
            Pause();
        }
        else {
            SceneManager.LoadScene("End");
        }
    }

    public void Pause() {
        timeScale = 0f;
        paused = true;
    }

    private static void ClearProjectiles() {
        Projectile[] projectiles = (Projectile[])GameObject.FindObjectsOfType(typeof(Projectile));
        for (int i = 0; i < projectiles.Length; i++) {
            Destroy(projectiles[i].gameObject);
        }
        Enemy[] enemies = (Enemy[])GameObject.FindObjectsOfType(typeof(Enemy));
        for (int i = 0; i < enemies.Length; i++) {
            Destroy(enemies[i].gameObject);
        }
    }

    public static void PlaySound(AudioClip audioClip, AudioSource audioSource, bool _override = false) {
        if (audioSource.clip == audioClip && audioSource.isPlaying && !_override) {
            return;
        }
        audioSource.clip = audioClip;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        audioSource.Play();
    }

    public static void PlaySound(AudioClip audioClip, int index = 1, bool _override = false) {
        AudioSource audioSource = Instance.GetComponent<AudioSource>();
        if (audioSource.isPlaying) {
            index = 2;
        }

        if (index == 2) {

            bool foundAudioSource = false;
            foreach (Transform child in Instance.transform) {
                if (child.GetComponent<AudioSource>() != null && !child.GetComponent<AudioSource>().isPlaying) {
                    audioSource = child.GetComponent<AudioSource>();
                    foundAudioSource = true;
                }
            }
            if (!foundAudioSource) {
                return;
            }

        }

        if (audioSource.clip == audioClip && audioSource.isPlaying && !_override) {
            return;
        }
        audioSource.clip = audioClip;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        audioSource.Play();
    }

    #endregion

}


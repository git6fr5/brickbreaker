/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Screen.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class Screen : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Instance.
    public static Screen Instance;

    // Components.
    [HideInInspector] public Camera mainCamera;
    [SerializeField] public PixelPerfectCamera pixelPerfectCamera;

    // Settings.
    [SerializeField, ReadOnly] private Vector2 screenSize;
    [SerializeField, ReadOnly] private Vector3 origin;

    // Post Processing.
    [SerializeField] public Volume volume;
    [SerializeField] public VolumeProfile volumeProfile;

    [Header("Shake")]
    [SerializeField] private AnimationCurve curve;
    [SerializeField, ReadOnly] public float shakeStrength = 1f;
    [SerializeField, ReadOnly] public float shakeDuration = 0.5f;
    [SerializeField, ReadOnly] float elapsedTime = 0f;
    [SerializeField, ReadOnly] public bool shake;

    // Callbacks.
    public static Vector2 ScreenSize => Instance.screenSize;

    #endregion

    /* --- Unity --- */
    #region Unity

    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        transform.position = origin;
        if (shake) {
            shake = Shake();
        }
    }

    #endregion

    /* --- Initialization --- */
    #region Initialization

    // Initializes this script.
    public void Init() {
        mainCamera = GetComponent<Camera>();
        volume.sharedProfile = null;
        origin = transform.position;
        screenSize = new Vector2(pixelPerfectCamera.refResolutionX, pixelPerfectCamera.refResolutionY) / pixelPerfectCamera.assetsPPU;
        Instance = this;
    }

    #endregion

    /* --- Shaking --- */
    #region Shaking

    public bool Shake() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= shakeDuration) {
            elapsedTime = 0f;
            return false;
        }
        float strength = shakeStrength * curve.Evaluate(elapsedTime / shakeDuration);
        transform.position += (Vector3)Random.insideUnitCircle * strength;
        return true;
    }

    public static void CameraShake(float strength, float duration) {
        if (strength == 0f) {
            return;
        }
        if (!Instance.shake) {
            Instance.shakeStrength = strength;
            Instance.shakeDuration = duration;
            Instance.shake = true;
        }
        else {
            Instance.shakeStrength = Mathf.Max(Instance.shakeStrength, strength);
            Instance.shakeDuration = Mathf.Max(Instance.shakeDuration, Instance.elapsedTime + duration);
        }
    }

    #endregion

    /* --- Debugging --- */
    #region Debugging

    void OnDrawGizmos() {
        Vector3 screenSize = new Vector3((float)pixelPerfectCamera.refResolutionX / pixelPerfectCamera.assetsPPU, (float)pixelPerfectCamera.refResolutionY / pixelPerfectCamera.assetsPPU, 0f);
        Gizmos.DrawWireCube(transform.position, screenSize);
        this.screenSize = screenSize;
    }

    #endregion


}

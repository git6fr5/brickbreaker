/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Spritesheet : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Controller controller;
    [SerializeField] private Sprite[] sprites;

    // Animation.
    [Space(2), Header("Animation")]
    [SerializeField] private bool animate = true;
    [SerializeField] private int indexOffset;
    [SerializeField] private int idleFrames;
    [SerializeField] private int movementFrames;
    [SerializeField] private int chargeFrames;
    [SerializeField] private int actionFrames;
    [HideInInspector] private Sprite[] idleAnimation;
    [HideInInspector] private Sprite[] movementAnimation;
    [HideInInspector] private Sprite[] actionAnimation;
    [HideInInspector] private Sprite[] currentAnimation;
    [HideInInspector] private Sprite[] previousAnimation;
    [SerializeField, ReadOnly] private int currentFrame;

    // Ticks.
    [Space(2), Header("Ticks")]
    [SerializeField, ReadOnly] private float ticks;
    [SerializeField, ReadOnly] private float frameRate;

    #endregion

    /* --- Unity --- */
    // Runs once before the first frame.
    void Start() {
        Init();
    }

    void Update() {
        float deltaTime = Time.deltaTime;
        ApplyEffect();
        if (animate) {
            Animate(deltaTime);
        }
        Rotate();
    }

    /* --- Methods --- */
    public void Init() {
        // Caching components.
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller>();
        frameRate = GameRules.FrameRate;
        Organize();
    }

    /* --- Animation --- */
    #region Animation
    private void Animate(float deltaTime) {
        currentAnimation = GetAnimation();
        ticks = previousAnimation == currentAnimation ? ticks + deltaTime : 0f;
        currentFrame = (int)Mathf.Floor(ticks * frameRate) % currentAnimation.Length;
        spriteRenderer.sprite = currentAnimation[currentFrame];
        previousAnimation = currentAnimation;
    }

    private void Rotate() {
        float angle = 180f * (int)controller.directionFlag;
        transform.localRotation = Quaternion.Euler(0f, angle, 0f);
    }

    // Gets the current animation info.
    public Sprite[] GetAnimation() {
        frameRate = GameRules.FrameRate;

        switch (controller.movementFlag) {
            case (Controller.Movement.Moving):
                frameRate = frameRate / 2;
                return movementAnimation;
            default:
                frameRate = frameRate / 2;
                return idleAnimation;
        }
    }

    // Organizes the sprite sheet into its animations.
    public void Organize() {
        int startIndex = indexOffset;
        startIndex = SliceSheet(startIndex, idleFrames, ref idleAnimation);
        startIndex = SliceSheet(startIndex, movementFrames, ref movementAnimation);
        startIndex = SliceSheet(startIndex, actionFrames, ref actionAnimation);
    }

    // Slices an animation out of the the sheet.
    private int SliceSheet(int startIndex, int length, ref Sprite[] array) {
        List<Sprite> splicedSprites = new List<Sprite>();
        for (int i = startIndex; i < startIndex + length; i++) {
            splicedSprites.Add(sprites[i]);
        }
        array = splicedSprites.ToArray();
        return startIndex + length;
    }

    #endregion

    /* --- Effects --- */
    #region Effects

    private void ApplyEffect() {
        HurtEffect();
    }

    private void HurtEffect() {
        spriteRenderer.material.SetFloat("_Hurt", controller.hurt ? 1f : 0f);
    }

    public void AfterImage(float delay, float transparency) {
        SpriteRenderer afterImage = new GameObject("AfterImage", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
        // afterImage.transform.SetParent(transform);
        afterImage.transform.position = transform.position;
        afterImage.transform.localRotation = transform.localRotation;
        afterImage.transform.localScale = transform.localScale;
        afterImage.sprite = spriteRenderer.sprite;
        afterImage.color = Color.white * transparency;
        afterImage.sortingLayerName = spriteRenderer.sortingLayerName;
        Destroy(afterImage.gameObject, delay);
    }

    #endregion

}

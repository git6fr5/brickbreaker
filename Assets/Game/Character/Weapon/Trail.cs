// Libraries.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Trail : MonoBehaviour {

    /* --- Variables --- */
    #region Variables

    // Components.
    [HideInInspector] private Weapon weapon;
    [HideInInspector] private LineRenderer lineRenderer;

    // Trail.
    [SerializeField] private float width;
    [SerializeField] public float fadeInterval;
    [SerializeField, ReadOnly] public List<Vector3> trail;

    #endregion

    /* --- Unity --- */
    #region Unity

    void Start() {
        weapon = transform.parent.GetComponent<Weapon>();
        lineRenderer = GetComponent<LineRenderer>();
        trail = new List<Vector3>();
    }

    void Update() {
        Capture();
        Render();
    }

    #endregion

    /* --- Trail --- */
    #region Trail

    private void Capture() {
        if (trail.Count <= 0) {
            trail.Insert(0, weapon.TipPosition);
            StartCoroutine(IEFade());
        }
        else if ((trail[0] - weapon.TipPosition).magnitude > GameRules.MovementPrecision) {
            trail.Insert(0, weapon.TipPosition);
            StartCoroutine(IEFade());
        }
        
    }

    private IEnumerator IEFade() {
        yield return new WaitForSeconds(fadeInterval);
        if (trail.Count > 0) {
            trail.RemoveAt(trail.Count - 1);
        }
    }

    #endregion

    /* --- Rendering --- */
    #region Rendering

    void Render() {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = 0f;
        lineRenderer.positionCount = trail.Count;
        lineRenderer.SetPositions(trail.ToArray());
    }

    #endregion

}

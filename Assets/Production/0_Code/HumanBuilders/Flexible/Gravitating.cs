﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// This behavior can be added to something that should gravitate towards another object. For example, see <see cref="GravitatingCurrency.cs" />
  /// </summary>
  public class Gravitating : MonoBehaviour {

    #region Variables
    #region Gravity Settings
    [Header("Gravity Settings", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// How quickly the object should accelerate towards its target. 0 - do not gravitate. 1 - snap to target.
    /// </summary>
    [Tooltip("How quickly the object should accelerate towards its target. 0 - do not gravitate. 1 - snap to target.")]
    [SerializeField]
    [Range(0,1)]
    private float gravitationStrength;

    /// <summary>
    /// How quickly rigidbody physics stops affecting gravitation. 0 - Cancel out normal physics immediately. 1 - Do not cancel out rigidbody physics.
    /// </summary>
    [Tooltip("How much rigidbody physics should play into the gravitation. 0 - Cancel out normal physics immediately. 1 - Do not cancel out rigidbody physics.")]
    [SerializeField]
    [Range(0,1)]
    private float rigidbodyDeceleration = 0.9f;

    [Space(10, order=2)]
    #endregion


    #region Target
    [Header("Target", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The object that this object should gravitate towards.
    /// </summary>
    [Tooltip("The object that this object should gravitate towards.")]
    [SerializeField]
    private GameObject target;
    #endregion


    #region Other Variables
    /// <summary>
    /// A caching variable used by Vector3.SmoothDamp().
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// A reference to the rigidbody of this component.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// The camera attached to the targetting camera.
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Where the object *should* be if it were explicitly in screen space.
    /// </summary>
    private Vector3 virtualScreenPos;
    #endregion
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      rb = GetComponent<Rigidbody2D>();
      cam = FindObjectOfType<TargettingCamera>().GetComponent<Camera>();
    }

    private void FixedUpdate() {
      if (target == null) {
        return;
      }
      Vector3 targScreenPos = cam.WorldToScreenPoint(target.transform.position);

      virtualScreenPos = Vector3.SmoothDamp(
        virtualScreenPos,
        targScreenPos,
        ref velocity,
        gravitationStrength
      );

      transform.position = cam.ScreenToWorldPoint(virtualScreenPos);
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set which game object to gravitate towards.
    /// </summary>
    /// <param name="target">The game object to gravitate towards.</param>
    public void GravitateTowards(GameObject target) {
      this.target = target;
      virtualScreenPos = cam.WorldToScreenPoint(transform.position);
    }

    /// <summary>
    /// Stop gravitating towards a particular target.
    /// </summary>
    public void StopGravitating() {
      target = null;
    }

    /// <summary>
    /// Sets how quickly this object will gravitate towards its target.
    /// </summary>
    /// <param name="strength">The gravitational strength. 0 - No gravitation. 1 - Snaps to target.</param>
    public void SetGravity(float strength) {
      gravitationStrength = Mathf.Clamp(strength, 0, 1);
    }

    /// <summary>
    /// Sets how quickly rigidbody physics stops affecting gravitation. 0 - Cancel out normal physics immediately. 1 - Do not cancel out rigidbody physics.
    /// </summary>
    /// <param name="deceleration"></param>
    public void SetRigidbodyDeceleration(float deceleration) {
      rigidbodyDeceleration = deceleration;
    }

    #endregion
  }

}
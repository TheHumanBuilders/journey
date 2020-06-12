using System;
using System.Collections.Generic;
using Storm.Characters.Player;
using Storm.Attributes;
using Unity;
using UnityEngine;

namespace Storm.LevelMechanics.Platforms {

  /// <summary>
  /// Platforms that players can hop onto from below.
  /// Pressing down lets the player through the platform.
  /// Use on a parent object with a composite collider.
  /// +----------------------+
  /// | +----+ +----+ +----+ |
  /// | |    | |    | |    | | 
  /// | +----+ +----+ +----+ |
  /// +----------------------+
  /// </summary>
  public class OneWayPlatform : MonoBehaviour {

    #region Variables

    #region Player Variables
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private static PlayerCharacter player;

    /// <summary>
    /// A reference to the player's collider.
    /// </summary>
    private static BoxCollider2D playerCollider;

    /// <summary>
    /// Whether or not the player is touching this platform.
    /// </summary>
    private bool playerIsTouching;

    #endregion


    #region Temporary Collider Disabling Variables
    /// <summary>
    /// The platform's collider.
    /// </summary>
    private BoxCollider2D platformCollider;


    /// <summary>
    /// A timer to keep the platform's collider disabled for a period.
    /// </summary>
    private float disableTimer;

    /// <summary>
    /// How long to disable the platform's collider when the player is trying to drop through.
    /// </summary>
    [Tooltip("How long to disable the platform's collider when the player is trying to drop through.")]
    public float disabledTime = 0.5f;


    /// <summary>
    /// Whether or not the player is trying to drop through the platform.
    /// </summary>
    [Tooltip("Whether or not the player is trying to drop through the platform.")]
    [SerializeField]
    [ReadOnly]
    private bool droppingThrough;

    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    public void Awake() {
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
        playerCollider = player.GetComponent<BoxCollider2D>();
      }

      platformCollider = GetComponent<BoxCollider2D>();
    }

    protected void Update() {
      if (playerIsTouching && Input.GetKeyDown(KeyCode.DownArrow)) {
        platformCollider.enabled = false;
        droppingThrough = true;
        disableTimer = disabledTime;
      }

      if (!platformCollider.enabled) {
        disableTimer -= Time.deltaTime;
      }

      if (droppingThrough && disableTimer < 0) {
        disableTimer = 0;
        platformCollider.enabled = true;
        droppingThrough = false;
        disableTimer = 0;
      }
    }

    protected void FixedUpdate() {
      // Bottom of player collider.
      float bottomOfPlayerCollider = playerCollider.bounds.center.y - playerCollider.bounds.extents.y;

      // Top of platformCollider
      float topOfPlatformCollider = platformCollider.bounds.center.y + platformCollider.bounds.extents.y;

      // The player is rising.
      bool ascending = player.IsRising();

      //TODO: The platform should only be disabled for the player,
      //      which probably means that the platform collider layer needs to change
      //      depending on the test below, with one layer being ignored by
      //      player collisions, instead of just disabling the collider alltogether. 
      //      Switch this over once enemies or dynamic/freebody obstacles become a thing.
      platformCollider.enabled = (bottomOfPlayerCollider >= topOfPlatformCollider) && !(droppingThrough || ascending);

      // Also, MAKE SURE THE ROTATION IS AT ZERO FOR OBJECTS WITH THIS SCRIPT.
    }

    void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        playerIsTouching = true;
      }
    }

    void OnCollisionExit2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player")) {
        playerIsTouching = false;
      }
    }

    #endregion
  }
}
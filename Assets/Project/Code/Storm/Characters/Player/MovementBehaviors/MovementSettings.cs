﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  public class MovementSettings : MonoBehaviour {

    #region Horizontal Movement
    [Header("Horizontal Movement", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The maximum speed the player can move horizontally.
    /// </summary>
    [Tooltip("The maximum speed the player can move horizontally.")]
    public float MaxSpeed = 44f;

    /// <summary>
    /// How quickly the player accelerates to max speed. Higher = faster acceleration.
    /// </summary>
    [Tooltip("How quickly the player accelerates to max speed. Higher = faster acceleration.")]
    [Range(0, 1)]
    public float Acceleration = 0.25f;

    /// <summary>
    /// How quickly the player decelerates. Higher = faster deceleration.
    /// </summary>
    [Tooltip("How quickly the player decelerates. Higher = faster deceleration.")]
    [Range(0, 1)]
    public float Deceleration = 0.2f;

    /// <summary>
    /// How quickly the player turns around while in motion. Higher = faster turn around time.
    /// </summary>
    [Tooltip("How quickly the player turns around while in motion. Higher = faster turn around time.")]
    public float Agility = 4f;

    /// <summary>
    /// How quickly the player can crawl. Acceleration and deceleration do not affect crawl speed.
    /// </summary>
    [Tooltip("How quickly the player can crawl. Acceleration and deceleration do not affect crawl speed.")]
    public float CrawlSpeed = 18f;

    /// <summary>
    /// The horizontal movement threshold at which the player is considered idle.
    /// </summary>
    [Tooltip("The horizontal movement threshold at which the player is considered idle.")]
    public float IdleThreshold = 0.05f;

    [Space(10, order=2)]
    #endregion

    #region Vertical Movement Settings
    [Header("Vertical Movement", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The strength of the player's first jump.
    /// </summary>
    [Tooltip("The strength of the player's first jump.")]
    public float SingleJumpForce = 48f;

    /// <summary>
    /// The strength of the player's second jump.
    /// </summary>
    [Tooltip("The strength of the player's second jump.")]
    public float DoubleJumpForce = 48f;


    /// <summary>
    /// The hop performed by the player when diving into a crawl.
    /// </summary>
    [Tooltip("The hop performed by the player when diving into a crawl.")]
    public Vector2 DiveHop;

    /// <summary>
    /// The strength of the player's wall jump (both vertical & horizontal forces).
    /// </summary>
    [Tooltip("The strength of the player's wall jump (both vertical & horizontal forces).")]
    public Vector2 WallJump;

    /// <summary>
    /// How easy it is for the player to get back to the wall after a
    /// wall jump. Higher is easier.
    /// </summary>
    [Tooltip("How easy it is for the player to get back to the wall after a wall jump. Higher is easier.")]
    public float WallJumpMuting = 0.08f;

    /// <summary>
    /// The speed the player runs up walls.
    /// </summary>
    [Tooltip("The speed the player runs up walls.")]
    public float WallRunSpeed = 0.125f;
    #endregion
  }
}
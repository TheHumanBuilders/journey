﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// When the player starts a jump from a wall.
  /// </summary>
  public class WallJump : HorizontalMotion {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "wall_jump";

    /// <summary>
    /// A reference to the targetting camera.
    /// </summary>
    private TargettingCamera cam;
    #endregion


    #region  Player State API
    /// <summary>
    /// First time initialization for the state. A reference to the player and the player's rigidbody will already have been added by this point.
    /// </summary>
    public override void OnStateAdded() {
      base.OnStateAdded();
      
      MovementSettings settings = GetComponent<MovementSettings>();
    }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public override void OnStateExit() {
      player.StartWallJumpMuting();

      float leftDist = player.DistanceToLeftWall();
      float rightDist = player.DistanceToRightWall();

      if (player.IsTouchingLeftWall() || leftDist < rightDist) {
        physics.Velocity = settings.WallJump;
      } else if (player.IsTouchingRightWall() || rightDist >= leftDist) {
        physics.Velocity = new Vector2(-settings.WallJump.x, settings.WallJump.y);
      }

      if (cam == null)  {
        cam = FindObjectOfType<TargettingCamera>();
      }

      if (cam != null) {
        TargettingCamera.ResetTracking(false, true);
      }
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnWallJumpFinished() {
      if (!exited) {
        ChangeToState<SingleJumpRise>();
      }
    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion
  }
}
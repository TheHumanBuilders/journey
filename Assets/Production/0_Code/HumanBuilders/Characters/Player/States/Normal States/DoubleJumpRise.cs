﻿using System.Collections;
using System.Collections.Generic;


using UnityEngine;


namespace HumanBuilders {

  /// <summary>
  /// WHen the player is rising from their double jump.
  /// </summary>
  public class DoubleJumpRise : HorizontalMotion {

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
    private string param = "jump_2_rise";
    #endregion

    #region Player State API

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingRightWall() || player.IsTouchingLeftWall()) {
        ChangeToState<WallSlide>();
      } else if (player.IsFalling()) {
        ChangeToState<DoubleJumpFall>();
      } else if (player.PressedJump()) {
        base.TryBufferedJump();
      } else if (player.PressedAction() || player.PressedAltAction()) {
        player.Interact();
      }
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        ChangeToState<CarryJumpRise>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }
    #endregion
  }

}
﻿using System.Collections;
using System.Collections.Generic;


using UnityEngine;


namespace HumanBuilders {

  /// <summary>
  /// When the player tosses an item in the middle of a jump or fall.
  /// </summary>
  public class MidAirThrowItem : HorizontalMotion {

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
    private string param = "carry_jump_throw";
    #endregion

    #region State API
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);
      
      if (player.CarriedItem == null) {
        ChangeToState<SingleJumpFall>();

      } else if (player.IsTouchingGround()) {
        if (player.TryingToMove()) {
          ChangeToState<Running>();
        } else {
          ChangeToState<Land>();
        }
      } else if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        if (player.IsRising()) {
          ChangeToState<WallRun>();
        } else  {
          ChangeToState<WallSlide>();
        }
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      player.Interact();
      
      if (player.IsTouchingGround()) {
        player.Physics.Vy = settings.SingleJumpForce;
      }

      if (player.HoldingAltAction()) {
        player.Drop(player.CarriedItem);
      } else if (player.HoldingAction()) {
        player.Throw(player.CarriedItem);
      } else {
        // Default action. :P Keep this separate from the other case, because
        // A.) it's easier to reason about written like this, and B.) this may
        // or may not change in the future. 
        player.Drop(player.CarriedItem);
      }


    }

    public override void OnSignal(GameObject obj) {
      if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnMidAirThrowItemFinished() {
      if (!exited) {
        if (player.IsRising()) {
          ChangeToState<SingleJumpRise>();
        } else {
          ChangeToState<SingleJumpFall>();
        }
      }
    }
    #endregion
  }
}
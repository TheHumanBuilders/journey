﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// When the player lands from a jump while carrying an item.
  /// </summary>
  public class CarryLand : CarryMotion {

    #region  Fields
    /// <summary>
    /// Whether or not the player has already exited this state.
    /// </summary>
    private bool exited;

    /// <summary>
    /// Whether or not the player has released the action button since entering
    /// this state.
    /// </summary>
    private bool releasedAction;
    #endregion


    #region Unity API
    private void Awake() {
      AnimParam = "carry_land";
    }

    #endregion

    #region State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<CarryJumpStart>();
      } else if (releasedAction && player.HoldingAction()) {
        ChangeToState<DropItem>();
      }

      if (player.ReleasedAction()) {
        releasedAction = true;
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      if (player.TryingToMove()) {
        ChangeToState<CarryRun>();
      }
    }

    /// <summary>
    /// Animation event hook
    /// </summary>
    public void OnCarryLandFinished() {
      if (!exited) {
        ChangeToState<CarryIdle>();
      }
    }

    public override void OnStateEnter() {
      releasedAction = player.ReleasedAction() || !player.HoldingAction();
    }

    public override void OnStateExit() {
      exited = true;
    }
    #endregion
  }

}
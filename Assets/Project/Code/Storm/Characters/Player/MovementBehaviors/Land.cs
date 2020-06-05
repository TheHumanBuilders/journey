﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// When the player lands from a light fall.
  /// </summary>
  public class Land : PlayerState {

    #region Unity API
    private void Awake() {
      AnimParam = "land";
    }
    #endregion

    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.HoldingDown()) {
        ChangeToState<CrouchStart>();
      } else if (player.PressedJump()) {
        ChangeToState<SingleJumpStart>();
      } else if (player.TryingToMove()) {
        ChangeToState<Running>();
      }
    }

    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }

    /// <summary>
    /// Animation pre-hook.
    /// </summary>
    public void OnLandFinished() {
      Debug.Log("Land!");
      ChangeToState<Idle>();
    }
    #endregion
  }
}

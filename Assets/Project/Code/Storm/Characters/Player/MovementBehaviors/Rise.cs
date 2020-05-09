﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Characters.Player {
  public class Rise : MovementBehavior {
    #region Fields

    private HorizontalMotion motion;

    private Rigidbody2D playerRB;
    #endregion


    private void Awake() {
      AnimParam = "rise";

      motion = GetComponent<HorizontalMotion>();
    }


    private void OnCollisionStay2D(Collision2D collision) {
      if (collision.otherCollider.CompareTag("Ground")) {
        Debug.Log("AAAAAAAAAAAAAAAAAAAA");
        if (player.IsTouchingLeftWall()) {
          player.SetFacing(Facing.Left);
          ChangeState<WallRun>();
        } else if (player.IsTouchingLeftWall()) {
          player.SetFacing(Facing.Right);
          ChangeState<WallRun>();
        }
      }
    }

    #region Movement Behavior Implementation
    //-------------------------------------------------------------------------
    // Movement Behavior Implementation
    //-------------------------------------------------------------------------

    public override void HandleInput() {
      if (Input.GetButtonDown("Jump")) {
        if (TryConsume(MovementSymbol.Jumped)) {
          ChangeState<DoubleJump>();
        }
      }
    }


    public override void HandlePhysics() {
      if (playerRB.velocity.y < 0) {
        ChangeState<Fall>();
      }

      Facing facing = motion.Handle();
      player.SetFacing(facing);
    }

    public override void OnStateEnter(PlayerCharacter p) {
      base.OnStateEnter(p);

      playerRB = GetComponent<Rigidbody2D>();
    }
    #endregion
  }
}
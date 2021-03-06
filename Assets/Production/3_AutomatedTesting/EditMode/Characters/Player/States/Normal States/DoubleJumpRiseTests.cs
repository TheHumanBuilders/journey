﻿using System.Collections;
using NSubstitute;
using NUnit.Framework;
using HumanBuilders;


using UnityEngine;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {
  public class DoubleJumpRiseTests : PlayerStateTest<DoubleJumpRise> {

    #region Unit Tests
    [Test]
    public void DJumpRise_Can_WallSlide() {
      SetupTest();

      player.IsTouchingLeftWall().Returns(true);
      
      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }

    [Test]
    public void DJumpRise_Can_Fall() {
      SetupTest();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<DoubleJumpFall>();
    }
    #endregion

    #region Integration Tests
    [Test]
    public void DJump_Can_BufferedJump() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsFalling().Returns(false);
      player.PressedJump().Returns(true);

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(1);

      state.OnFixedUpdate();

      AssertStateChange<SingleJumpStart>();
    }
    #endregion

  }
}
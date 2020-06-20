﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;

namespace Tests.Characters.Player {
  public class CarryJumpStartTests : PlayerStateTest<CarryJumpStart> {
    

    [Test]
    public void Can_Throw_Item_In_Midair() {
      SetupTest();
      
      player.PressedAction().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<MidAirThrowItem>();
    }

    [Test]
    public void Can_Rise_While_Carrying() {
      SetupTest();
      
      state.OnCarryJumpStartFinished();
      
      AssertStateChange<CarryJumpRise>();
    }

    [Test]
    public void Force_Applied() {
      SetupTest();

      carrySettings.JumpForce = 1;
      physics.Velocity = Vector2.zero;

      state.OnStateExit();

      Assert.AreEqual(1, physics.Vy);
    }
  }
}
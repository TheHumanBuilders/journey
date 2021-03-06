﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;

namespace HumanBuilders.Tests {
  public class CarryCrouchingTests : PlayerStateTest<CarryCrouching> {


    [Test]
    public void Can_End_Crouch_While_Carrying() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      player.HoldingDown().Returns(false);
      state.OnUpdate();

      AssertStateChange<CarryCrouchEnd>();
    }
  }
}
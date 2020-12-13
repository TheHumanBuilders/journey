﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {

  /// <summary>
  /// This component is used during in-game cutscenes to mimick dialog during a
  /// timeline animation.
  /// </summary>
  [RequireComponent(typeof(Message))]
  public class CutsceneDialog : MonoBehaviour {

    /// <summary>
    /// Start a dialog from the middle of a cutscene.
    /// </summary>
    /// <seealso cref="DialogManager.Type" />
    public void Type(Message message) {
      if (!DialogManager.IsDialogBoxOpen()) {
        DialogManager.OpenDialogBox();
      }

      DialogManager.Type(message.Sentence, message.Speaker);
    }

  }

}
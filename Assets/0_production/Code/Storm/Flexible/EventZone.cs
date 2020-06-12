﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Storm.Flexible {

  /// <summary>
  /// This behavior can be added to an area game object to singal that a set of events should happen when the player enters the area.
  /// </summary>
  public class EventZone : MonoBehaviour {

    #region Variables
    /// <summary>
    /// The list of events that will fire when the player triggers this game object.
    /// </summary>
    [Tooltip("The list of events that will fire when the player triggers this game object.")]
    public UnityEvent Events;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    public void OnTriggerEnter2D(Collider2D col) {
      if (col.gameObject.CompareTag("Player")) {
        Events.Invoke();
      }
    }
    #endregion
  }
}
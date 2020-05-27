﻿using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// This behavior disables moving for the player character while they are within the collision area of the game object.
  /// </summary>
  /// <seealso cref="NoJumpZone" />
  public class NoMoveZone : MonoBehaviour {

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    
    public void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().DisableMove();
      }
    }

    public void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        other.GetComponent<PlayerCharacter>().EnableMove();
      }
    }

    #endregion
  }
}
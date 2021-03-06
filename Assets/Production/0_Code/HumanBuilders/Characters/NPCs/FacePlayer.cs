﻿using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A script that causes the NPC its placed on to always face the player.
  /// </summary>
  public class FacePlayer : MonoBehaviour {

    #region Variables
    /// <summary>
    /// A reference to the game object sprite.
    /// </summary>
    private SpriteRenderer sprite;

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private static PlayerCharacter player;
    #endregion

    #region  Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------

    // Start is called before the first frame update
    private void Start() {
      sprite = GetComponent<SpriteRenderer>();
      player = FindObjectOfType<PlayerCharacter>();
    }

    // Update is called once per frame
    private void Update() {
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }
      
      if (transform.position.x > player.transform.position.x && !sprite.flipX) {
        sprite.flipX = true;
        transform.localScale.Set(-1, transform.localScale.y, transform.localScale.z);
      } else if (transform.position.x <= player.transform.position.x && sprite.flipX) {
        sprite.flipX = false;
        transform.localScale.Set(1, transform.localScale.y, transform.localScale.z);
      }
    }

    private void OnDisable() {
      if (sprite != null) {
        sprite.flipX = false;
      }
      transform.localScale = Vector3.one;
    }
    #endregion
  }
}
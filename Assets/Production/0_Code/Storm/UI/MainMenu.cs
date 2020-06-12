﻿using System.Collections;
using System.Collections.Generic;
using Storm.Subsystems.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.UI {

  /// <summary>
  /// The behavior for the Game's title menu.
  /// </summary>
  /// <seealso cref="LevelSelect" />
  public class MainMenu : MonoBehaviour {


    #region Variables
    [Header("Starting Scene Information", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The name of the scene to load.
    /// </summary>
    [Tooltip("The name of the scene to load.")]
    [SerializeField]
    private string sceneName = "";

    /// <summary>
    /// The name of the spawn position for the player.
    /// </summary>
    [Tooltip("The name of the spawn position for the player.")]
    [SerializeField]
    private string spawnName = "";
    
    #endregion



    #region  Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Start playing the game.
    /// </summary>
    public void PlayGame() {
      TransitionManager.Instance.MakeTransition(sceneName, spawnName);
    }

    /// <summary>
    /// Quit playing the game.
    /// </summary>
    public void QuitGame() {
      Debug.Log("Quitting!");
      Application.Quit();
    }

    #endregion
  }
}
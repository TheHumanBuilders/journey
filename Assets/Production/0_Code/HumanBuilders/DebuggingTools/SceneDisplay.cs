﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace HumanBuilders {
  /// <summary>
  /// This is a utility behavior that displays the current name of the scene in game.
  /// </summary>
  public class SceneDisplay : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The text UI element used to display the scene's name.
    /// </summary>
    [SerializeField]
    [Tooltip("The text UI element used to display the scene's name.")]
    private TextMeshProUGUI text = null;

    /// <summary>
    /// The toggle button for this script.
    /// </summary>
    [SerializeField]
    [Tooltip("The toggle button for this script.")]
    private UnityEngine.UI.Toggle toggle = null;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      if (toggle != null) {
        if (!toggle.isOn) {
          enabled = false;
        }
      }
    }

    private void OnEnable() {
      SceneManager.sceneUnloaded += OnSceneUnloaded;
      DisplaySceneName();
    }

    private void OnDisable() {
      HideSceneName();
      SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void Toggle() {
      enabled = !enabled;
    }

    /// <summary>
    /// Callback for scene loading.
    /// </summary>
    /// <param name="scene">The scene that's been loaded.</param>
    /// <param name="mode">The loading mode for the scene (either Single or Additive).</param>
    private void OnSceneUnloaded(Scene scene) {
      DisplaySceneName();
    }

    /// <summary>
    /// Display the name of the current scene.
    /// </summary>
    private void DisplaySceneName() {
      if (text != null) {
        text.text = SceneManager.GetActiveScene().name + "  ";
      }
    }

    /// <summary>
    /// Hide the name of the current scene.
    /// </summary>
    private void HideSceneName() {
      if (text != null) {
        text.text = "";
      }
    }
  }

}
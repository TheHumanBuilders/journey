﻿


using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders {

  public class PauseScreen : Singleton<PauseScreen> {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    public static bool Paused { get { return paused; } }

    /// <summary>
    /// The the game object that hosts the pause screen.
    /// </summary>
    private GameObject pauseScreen;

    /// <summary>
    /// Whether or not the game is paused.
    /// </summary>
    private static bool paused = false;

    /// <summary>
    /// The button for resuming the game. Set this in the inspector
    /// </summary>
    [Tooltip("The button for resuming the game.")]
    public MenuButton resumeButton;

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected new void Awake() {
      pauseScreen = transform.GetChild(0).gameObject;
      base.Awake();
      pauseScreen.SetActive(false);
      paused = false;
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        if (SceneManager.GetActiveScene().name != "main_menu") {
          if (pauseScreen.activeSelf) {
            ContinueGame();
          } else {
            PauseGame();
          }
        }
      }
    }


    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Pause the game!
    /// </summary>
    public void PauseGame() {
      if (pauseScreen != null) {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        paused = true;
      }
    }

    /// <summary>
    /// Unpause the game!
    /// </summary>
    public void ContinueGame() {
      if (pauseScreen != null) {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        paused = false;
      }
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMainMenu() {
      Time.timeScale = 1;
      TransitionManager.MakeTransition("main_menu");
      pauseScreen.SetActive(false);
      DialogManager.EndDialog();
      paused = false;
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void ExitApplication() {
      Application.Quit();
    }

    #endregion
  }
}

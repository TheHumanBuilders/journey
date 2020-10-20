﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Characters.Player;
using Storm.Extensions;
using UnityEngine;

namespace Storm.UI {

  public class Mouse : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The default mouse icon.
    /// </summary>
    [Tooltip("The default mouse icon.")]
    public Texture2D DefaultIcon;

    /// <summary>
    /// The list of custom mouse icons that can be used. This is just a
    /// convenience variable for the inspector.
    /// </summary>
    [Tooltip("The list of custom mouse icons that can be used.")]
    [SerializeField]
    [TableList]
    public List<MouseIcon> Icons = null;

    /// <summary>
    /// The table of Icons that can be used.
    /// </summary>
    private Dictionary<string, MouseIcon> iconTable;


    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerCharacter player;

    /// <summary>
    /// Whether or not the mouse is visible.
    /// </summary>
    [Tooltip("Whether or not the mouse is visible.")]
    [SerializeField]
    public bool visible = true;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      Cursor.SetCursor(DefaultIcon, new Vector2(16, 16), CursorMode.ForceSoftware);

      iconTable = new Dictionary<string, MouseIcon>();

      foreach (MouseIcon icon in Icons) {
        Debug.Log(icon.Sprite);
        iconTable.Add(icon.Name, icon);
      }
    }

    /// <summary>
    /// Swap in a different mouse icon.
    /// </summary>
    /// <param name="name">The name of the icon to use.</param>
    public void Swap(string name) {
      if (iconTable.ContainsKey(name)) {
        iconTable[name].Swap();
      }
    }

    #endregion

  }

}

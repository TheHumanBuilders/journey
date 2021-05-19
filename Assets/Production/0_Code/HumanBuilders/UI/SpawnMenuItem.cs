﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HumanBuilders {

  public class SpawnMenuItem : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public string Spawn { get { return spawn; }}

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Tooltip("The text object to populate with the spawn name.")]
    [SerializeField]
    private TextMeshProUGUI textMesh = null;
    private string spawn;

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void SetSpawn(string spawn) {
      this.spawn = spawn;
      if (textMesh != null) {
        textMesh.text = spawn;
      }
    }
  }

}
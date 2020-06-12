﻿using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEditor;
using UnityEngine;

namespace Storm.Editor {


  /// <summary>
  /// An editor GUI element for drawing read only properties.
  /// </summary>
  [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
  public class ReadOnlyDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property,
      GUIContent label) {
      return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
      SerializedProperty property,
      GUIContent label) {
      GUI.enabled = false;
      EditorGUI.PropertyField(position, property, label, true);
      GUI.enabled = true;
    }
  }
}
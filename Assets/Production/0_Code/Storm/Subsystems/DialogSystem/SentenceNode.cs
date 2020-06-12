﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {
  /// <summary>
  /// A dialog node representing a single screen of text with a speaker.
  /// </summary>
  [NodeWidth(360)]
  [NodeTint("#4a6d8f")]
  [CreateNodeMenu("Dialog/Basic/Sentence Node")]
  public class SentenceNode : Node {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The person saying the sentence.
    /// </summary>
    public string Speaker;

    [Space(8, order=1)]
    [TextArea(3,10)]

    /// <summary>
    /// The text being spoken.
    /// </summary>
    public string Text;

    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }
  }
}
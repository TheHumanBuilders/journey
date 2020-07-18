using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node representing list of decisions.
  /// </summary>
  [NodeTint("#996e39")]
  [NodeWidth(360)]
  [CreateNodeMenu("Dialog/Dynamic/Decision Node")]
  public class DecisionNode : DialogNode {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(12, order=0)]

    /// <summary>
    /// The list of decisions the player can make.
    /// </summary>
    [Output(dynamicPortList=true)]
    public List<string> Decisions;

    /// <summary>
    /// Whether or not to save the previous decision made at this node.
    /// </summary>
    [Tooltip("Whether or not to save the previous decision made at this node.")]
    public bool SaveDecision;

    /// <summary>
    /// The previous decision made at this node.
    /// </summary>
    private int prevDecisionIndex = 0;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }


    /// <summary>
    /// Get the index of the previous decision made at this node.
    /// </summary>
    public int GetPreviousDecision() {
      return SaveDecision ? prevDecisionIndex : 0;
    }

    /// <summary>
    /// Set the index of the previous decision at this node.
    /// </summary>
    /// <param name="decisionIndex">The index of the decision made.</param>
    public void SetPreviousDecision(int decisionIndex) {
      prevDecisionIndex = decisionIndex;
    }

    public override void HandleNode() {
      if (manager == null) {
        manager = DialogManager.Instance;
      }
      
      List<GameObject> decisionButtons = manager.GetDecisionButtons();
      
      int i = 0;
      if (decisionButtons != null) {
        for (i = 0; i < decisionButtons.Count; i++) {
          if (decisionButtons[i] == EventSystem.current.currentSelectedGameObject) {
            break;
          }
        } 
      }

      SetPreviousDecision(i);
      manager.ClearDecisions();
      manager.SetCurrentNode(GetNextNode());
      manager.ContinueDialog();
    }


    public override IDialogNode GetNextNode() {
      NodePort outputPort = GetOutputPort("Decisions "+prevDecisionIndex);
      NodePort inputPort = outputPort.Connection;
      
      return (IDialogNode)inputPort.node;
    }

    
  }
}
using XNode;

using UnityEngine;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A dialog node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Dialog/Animation/Animation Float")]
  public class AnimationFloatNode : DialogNode {

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The Animator controller to change.
    /// </summary>
    [Tooltip("The Animator controller to change.")]
    [SerializeField]
    public Animator Animator;

    /// <summary>
    /// The name of the parameter to set.
    /// </summary>
    [Tooltip("The name of the parameter to set.")]
    [SerializeField]
    public string Parameter;

    /// <summary>
    /// The float to set.
    /// </summary>
    [Tooltip("The float to set.")]
    [SerializeField]
    public float Value;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    #endregion

    #region XNode API
    //---------------------------------------------------
    // XNode API
    //---------------------------------------------------

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) => null;

    #endregion

    #region Dialog Node API
    public override void Handle() {      
      if (Animator == null) {
        if (player == null) {
          player = GameManager.Instance.player;
        }

        Animator = player.GetComponent<Animator>();
      }

      Animator.SetFloat(Parameter, Value);
    }

    #endregion
  }
}
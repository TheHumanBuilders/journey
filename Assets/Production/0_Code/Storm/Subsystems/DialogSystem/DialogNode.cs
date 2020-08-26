using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// The base class for Dialog Nodes. Defines the HandleNode() and GetNextNode API.
  /// </summary>
  public abstract class DialogNode : Node, IDialogNode {

    #region Fields
    //---------------------------------------------------------------------
    // Fields
    //---------------------------------------------------------------------
      
    /// <summary>
    /// A reference to the Dialog Manager.
    /// </summary>
    protected static DialogManager manager;

    /// <summary>
    /// A reference to the Player.
    /// </summary>
    protected static PlayerCharacter player;
    #endregion
      
    #region Dependency Injection
    //---------------------------------------------------------------------
    // Dependency Injection
    //---------------------------------------------------------------------
      
    /// <summary>
    /// Injection point for the dialog manager.
    /// </summary>
    /// <param name="manager">The dialog manager to inject.</param>
    public void Inject(DialogManager manager) {
      DialogNode.manager = manager;
    }
    
    #endregion
      
    #region XNode API
    //---------------------------------------------------------------------
    // XNode API
    //---------------------------------------------------------------------
    
    public override object GetValue(NodePort port) {
      return null;
    }

    #endregion
      
    #region Dialog Node API
    //---------------------------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------------------------
      
    public virtual void HandleNode() {
      if (manager == null) {
        manager = DialogManager.Instance;
      }

      if (!manager.HandlingConversation) {
        if (!manager.HandlingLock) {
          manager.HandlingConversation = true;
        }


        Handle();

        if (!manager.HandlingLock) {
          manager.HandlingConversation = false;
        }
      }

      PostHandle();
    }

    /// <summary>
    /// How to handle this node.
    /// </summary>
    public virtual void Handle() {

    }

    /// <summary>
    /// What to do after handling this node.
    /// </summary>
    public virtual void PostHandle() {
      manager.SetCurrentNode(GetNextNode());
      manager.ContinueDialog();
    }

    /// <summary>
    /// Get the next node.
    /// </summary>
    /// <returns></returns>
    public virtual IDialogNode GetNextNode() {
      return (IDialogNode)GetOutputPort("Output").Connection.node;
    }
    #endregion
  }
}

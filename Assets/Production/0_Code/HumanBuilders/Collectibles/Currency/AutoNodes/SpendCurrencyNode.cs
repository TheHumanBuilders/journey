using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A node that causes the player to spend some currency.
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Currency/Spend Currency")]
  public class SpendCurrencyNode : AutoNode {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The name of the currency to spend.
    /// </summary>
    [Tooltip("The name of the currency to spend.")]
    [ValueDropdown("Currencies")]
    public string Currency = "-- select a curreny --";

    /// <summary>
    /// The amount to spend.
    /// </summary>
    [Tooltip("The amount to spend.")]
    public float Amount;


    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node given that the player had enough
    /// money to spend.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Success;

    /// <summary>
    /// Output connection for the next node given that the player did not have
    /// enough money to spend.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Failure;

    /// <summary>
    /// Whether or not the payment succeeded.
    /// </summary>
    private bool succeeded = true;

    #endregion

    #region Dialog Node API
    //-------------------------------------------------------------------------
    // Dialog Node API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Try to spend an amount of currency.
    /// </summary>
    public override void Handle(GraphEngine graphEngine) {
      if (IsCurrencySelected() && player.GetCurrencyTotal(Currency) >= Amount) {
        player.SpendCurrency(Currency, Amount);
        succeeded = true;
      } else {
        succeeded = false;
      }
    }

    public override IAutoNode GetNextNode() {
      string name = succeeded ? "Success" : "Failure";
      return (IAutoNode)GetOutputPort(name).Connection.node;
    }

    #endregion


    #region Odin Inspector
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------

    private IEnumerable Currencies() {
      List<string> currencies = new List<string>();

      Wallet[] wallets = FindObjectsOfType<Wallet>();
      foreach (Wallet w in wallets) {
        currencies.Add(w.GetCurrencyName());
      }

      return currencies;
    }

    private bool IsCurrencySelected() {
      return Currency != "-- select a curreny --";
    }

    #endregion
  }
}
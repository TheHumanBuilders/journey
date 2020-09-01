

using System.Collections.Generic;

namespace Storm.Collectibles.Currency {
  public interface IInventory {
    
    /// <summary>
    /// Add currency of a particular type to the player's total.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <param name="amount">The amount to add.</param>
    /// <seealso cref="PlayerInventory.AddCurrency" />
    void AddCurrency(string name, float amount);

    /// <summary>
    /// Spend some currency of a particular type.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <param name="amount">The amount to spend.</param>
    /// <returns>
    /// True if the the player had enough currency to spend. 
    /// Otherwise, returns false and no currency is removed.
    /// </returns>
    /// <seealso cref="PlayerInventory.SpendCurrency" />
    bool SpendCurrency(string name, float amount);

    /// <summary>
    /// Get the total for a particular currency.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <returns>The amount of currency the player has of that type.</returns>
    /// <seealso cref="PlayerInventory.GetCurrencyTotal" />
    float GetCurrencyTotal(string name);
  }

  /// <summary>
  /// The Player's Inventory.
  /// </summary>
  public class PlayerInventory : IInventory {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Currencies that the player is carrying.  
    /// Key - The name of the currency.  
    /// Value - How much the player has of that currency.
    /// </summary>
    private Dictionary<string, float> currencies;

    #endregion

    #region Constructors
    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------
    public PlayerInventory() {
      currencies = new Dictionary<string, float>();
    }

    #endregion


    #region Currency
    //-------------------------------------------------------------------------
    // Currency
    //-------------------------------------------------------------------------

    /// <summary>
    /// Add currency of a particular type to the player's total.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <param name="amount">The amount to add.</param>
    public void AddCurrency(string name, float amount) {
      if (currencies.ContainsKey(name)) {
        currencies[name] += amount;
      } else {
        currencies.Add(name, amount);
      }
    }

    /// <summary>
    /// Spend some currency of a particular type.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <param name="amount">The amount to spend.</param>
    /// <returns>
    /// True if the the player had enough currency to spend. 
    /// Otherwise, returns false and no currency is removed.
    /// </returns>
    public bool SpendCurrency(string name, float amount) {
      if (currencies.ContainsKey(name)) {
        if (currencies[name] >= amount) {
            currencies[name] -= amount;
            return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Get the total for a particular currency.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <returns>The amount of currency the player has of that type.</returns>
    public float GetCurrencyTotal(string name) {
      if (currencies.ContainsKey(name)) {
        return currencies[name];
      } else {
        return 0;
      }
    }


    public bool ContainsCurrency(string name) {
      return currencies.ContainsKey(name);
    }

    #endregion

  }
}
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A single attack used by one of the game's bosses.
  /// </summary>
  [Serializable]
  public class BossAttack {
    /// <summary>
    /// How often this attack should fire, relative to the boss' other attacks.
    /// Though any value can work, it's expected that this number will be between 0-1 or 0-100, 
    /// with the frequencies of all attacks adding up to either 1 or 100.
    /// </summary>
    [LabelText("%")]
    [TableColumnWidth(80, Resizable = false)]
    [Range(0, 100)]
    public float Frequency;

    /// <summary>
    /// The name of this attack.
    /// </summary>
    [SuffixLabel("Stylized Name", true)]
    [LabelText(" ")]
    [LabelWidth(1)]
    [HorizontalGroup("Attack Info")]
    public string Name;

    /// <summary>
    /// The animation trigger parameter that causes this attack.
    /// </summary>
    [SuffixLabel("Trigger Parameter", true)]
    [LabelText(" ")]
    [LabelWidth(10)]
    [HorizontalGroup("Attack Info")]
    public string Trigger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">The name of the attack.</param>
    /// <param name="frequency">How likely it is for this attack to fire.</param>
    /// <param name="trigger">The name of the animation trigger parameter for this attack.</param>
    public BossAttack(string name, float frequency, string trigger) {
      Name = name;
      Frequency = frequency;
      Trigger = trigger;
    }
  }
}
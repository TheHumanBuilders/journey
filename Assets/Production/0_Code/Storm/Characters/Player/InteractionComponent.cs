using System.Collections.Generic;
using Storm.Attributes;
using Storm.Flexible.Interaction;
using Storm.Subsystems.FSM;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// Interface for the class responsible for handling player-environment interaciton.
  /// </summary>
  public interface IInteractionComponent {
    /// <summary>
    /// The current indicator.
    /// </summary>
    /// <seealso cref="InteractionComponent.CurrentIndicator" />
    Indicator CurrentIndicator { get; }

    /// <summary>
    /// Register an indicator to the cache.
    /// </summary>
    /// <param name="indicator">The indicator prefab to register. The first time the indicator is
    /// actually used in the scene, the prefab will be instantiated and the
    /// instance will be cached in its place.</param>
    /// <seealso cref="InteractionComponent.RegisterIndicator" />
    void RegisterIndicator(Indicator indicator);

    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="InteractionComponent.AddInteractible" />
    /// <param name="interactible">The object to add.</param>
    void AddInteractible(Interactible interactible);

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="InteractionComponent.RemoveInteractible" />
    /// <param name="interactible">The object to remove.</param>
    void RemoveInteractible(Interactible interactible);

    /// <summary>
    /// Interact with the closest interactible object.
    /// </summary>
    /// <seealso cref="InteractionComponent.Interact" />
    void Interact();
  }

  /// <summary>
  /// The delegate component for dealing with environment interactions.
  /// </summary>
  public class InteractionComponent : MonoBehaviour, IInteractionComponent {

    #region Properties
    /// <summary>
    /// The current indicator.
    /// </summary>
    public Indicator CurrentIndicator {
      get { return currentIndicator; }
    }
    #endregion

    #region Fields
    #region Player information
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private PlayerCharacter player;

    /// <summary>
    /// The player's collider.
    /// </summary>
    private Collider2D playerCollider;

    /// <summary>
    /// The player's state machine.
    /// </summary>
    private FiniteStateMachine fsm;

    /// <summary>
    /// The player's sprite renderer
    /// </summary>
    private SpriteRenderer playerSprite;
    #endregion

    #region Indicator Fields
    /// <summary>
    /// A pool of indicator instances. Every interactible will try to register
    /// their indicator to this cache.
    /// </summary>
    private Dictionary<string, Indicator> indicators;


    /// <summary>
    /// The currently active interaction indicator.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Indicator currentIndicator;

    /// <summary>
    /// The sprite renderer for the current interaction indicator.
    /// </summary>
    private SpriteRenderer currentIndicatorSprite;
    #endregion

    #region Interactible Fields
    /// <summary>
    /// The list of interactible objects that are close enough to the player to
    /// be interacted with.
    /// </summary>
    private List<Interactible> interactibles;

    /// <summary>
    /// The interactive object that's currently the closest to the player. If
    /// this is null, there's no interactive object that's close enough to be
    /// interacted with.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Interactible currentInteractible;

    /// <summary>
    /// The current interactible's sprite.
    /// </summary>
    private SpriteRenderer currentInteractibleSprite;
    #endregion
    #endregion

    #region Constructors
    private void Start() {
      player = GetComponent<PlayerCharacter>();
      playerCollider = player.GetComponent<Collider2D>();
      fsm = player.GetComponent<FiniteStateMachine>();
      Debug.Log("FSM: " + fsm);
      playerSprite = player.GetComponent<SpriteRenderer>();

      indicators = new Dictionary<string, Indicator>();
      interactibles = new List<Interactible>();
    }
    #endregion

    #region Unity API
    private void Update() {
      if (currentInteractible != null && currentInteractible.StillInteracting) {
        currentIndicatorSprite.enabled = false;
        return;
      }

      if (interactibles.Count > 0) {
        Interactible interactible = GetClosest();

        if (currentInteractible != interactible) {
          UpdateCurrentInteractible(interactible);
          Debug.Log("Updating closest to \"" + interactible.name + ".\"");
          UpdateCurrentIndicator();
        }

        UpdateIndicatorVisibility();
      }
    }

    #region Searching Over Interactibles
    /// <summary>
    /// Retrieves the closest interactible object that the player is close enough to interact with.
    /// </summary>
    /// <returns>The closest interactible object.</returns>
    private Interactible GetClosest() {
      if (interactibles.Count == 0) {
        return null;
      }

      Interactible closest = interactibles[0];
      float closestDist = DistanceFromPlayer(closest);
      foreach(Interactible next in interactibles) {
        float dist = DistanceFromPlayer(next);
        if (dist < closestDist) {
          closest = next;
          closestDist = dist;
        }
      }

      return closest;
    }

    /// <summary>
    /// How far away an interactible is from the player.
    /// </summary>
    /// <param name="interactible">The interactible to check.</param>
    /// <returns>The distance from the center of the player to the center of the
    /// interactible.</returns>
    private float DistanceFromPlayer(Interactible interactible) {
      return (interactible.Center - (Vector2)playerCollider.bounds.center).magnitude;
    }
    #endregion


    /// <summary>
    /// Update whether or not the indicator should be visiable.
    /// </summary>
    private void UpdateIndicatorVisibility() {
      currentIndicatorSprite.enabled = currentInteractible.ShouldShowIndicator();
    }

    #region Indicator Handling
    /// <summary>
    /// Update the current indicator information.
    /// </summary>
    private void UpdateCurrentIndicator() {
      RemoveCurrentIndicator();
      SetCurrentIndicator();
    }

    /// <summary>
    /// Set information about the current indicator.
    /// </summary>
    private void SetCurrentIndicator() {
      Transform parent = currentInteractible.IndicateOverPlayer ? player.transform : currentInteractible.transform;
      SpriteRenderer targetSprite = currentInteractible.IndicateOverPlayer ? playerSprite : currentInteractibleSprite;

      // Set the current indicator.
      currentIndicator = GetIndicator(currentInteractible.IndicatorName);

      // Set the indicator on the proper parent.
      currentIndicator.transform.parent = parent;
      currentIndicator.transform.localPosition = Vector2.zero;

      // move the offset of the indicator if necessary.
      currentIndicator.transform.localPosition += (Vector3)currentInteractible.Offset;

      // make the indicator visible.
      currentIndicatorSprite = currentIndicator.GetComponent<SpriteRenderer>();
      currentIndicatorSprite.enabled = currentInteractible.ShouldShowIndicator();

      // put the indicator on the same render layer as the object it's attached to.
      currentIndicatorSprite.sortingLayerName = targetSprite.sortingLayerName;
      currentIndicatorSprite.sortingOrder = targetSprite.sortingOrder+1;
    }


    /// <summary>
    /// Get the instance of the indicator with a given name.
    /// </summary>
    /// <param name="name">The name of the indicator to get.</param>
    /// <returns>The indicator with the given name. Instantiates the indicator</returns>
    private Indicator GetIndicator(string name) {
      if (indicators.ContainsKey(name)) {
        Indicator indicator = indicators[name];

        if (indicator.Instantiated) {
          return indicator;
        }

        GameObject go = Instantiate(
          indicator.gameObject,
          Vector2.zero,
          Quaternion.identity
        );

        Indicator instanced = go.GetComponent<Indicator>();
        instanced.Instantiated = true;

        indicators.Remove(name);
        indicators.Add(name, instanced);

        return instanced;

      } else { 
        throw new UnityException("No such indicator \"" + name + "\" exists in the scene!");
      }
    }

    /// <summary>
    /// Remove the current indicator.
    /// </summary>
    private void RemoveCurrentIndicator() {
      if (currentIndicator != null) {
        currentIndicator.transform.parent = null;
        currentIndicator = null;
        currentIndicatorSprite.enabled = false;
        currentIndicatorSprite = null;
      }
    }
    #endregion

    #region Interactible Handling
    /// <summary>
    /// Update the current interactible information.
    /// </summary>
    /// <param name="interactible">The interactible to set.</param>
    private void UpdateCurrentInteractible(Interactible interactible) {
      RemoveCurrentInteractible();
      SetCurrentInteractible(interactible);
    }
    
    /// <summary>
    /// Remove the current interactible.
    /// </summary>
    private void RemoveCurrentInteractible() {
      if (currentIndicator != null) {
        currentInteractible = null;
        currentInteractibleSprite = null;
      }
    }

    /// <summary>
    /// Set information about the current interactible.
    /// </summary>
    /// <param name="interactible">The interactible to set.</param>
    private void SetCurrentInteractible(Interactible interactible) {
      currentInteractible = interactible;
      currentInteractibleSprite = currentInteractible.GetComponent<SpriteRenderer>();
    }
    #endregion
    #endregion

    #region Public Interface

    /// <summary>
    /// Register an indicator to the cache.
    /// </summary>
    /// <param name="indicator">The indicator prefab to register. The first time the indicator is
    /// actually used in the scene, the prefab will be instantiated and the
    /// instance will be cached in its place.</param>
    public void RegisterIndicator(Indicator indicator) {
      if (!indicators.ContainsKey(indicator.Name)) {
        indicators.Add(indicator.Name, indicator);
      }
    }

    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to add.</param>
    public void AddInteractible(Interactible interactible) {
      interactibles.Add(interactible);
      Debug.Log("Adding \"" + interactible + ".\" Count: " + interactibles.Count + ".");
    }

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to remove.</param>
    public void RemoveInteractible(Interactible interactible) {
      interactibles.Remove(interactible);
      if (interactibles.Count == 0) {
        RemoveCurrentInteractible();
        RemoveCurrentIndicator();
      }
      Debug.Log("Removing \"" + interactible + ".\" Count: " + interactibles.Count + ".");
    }

    /// <summary>
    /// Interact with the closest interactible object.
    /// </summary>
    public void Interact() {
      if (CanInteract()) {
        currentInteractible.OnInteract();

        if (currentInteractible != null) {
          fsm.Signal(currentInteractible.gameObject);
        }
      }
    }

    /// <summary>
    /// Whether or not you can interact with the current object.
    /// </summary>
    private bool CanInteract() {
      if (currentInteractible == null) {
        return false;
      }

      if (currentIndicatorSprite == null) {
        return false;
      }

      if (fsm == null) {
        return false;
      }

      return currentInteractible.StillInteracting || currentIndicatorSprite.enabled;
    }

    #endregion
  }
}
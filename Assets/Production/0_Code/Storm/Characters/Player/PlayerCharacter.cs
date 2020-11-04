﻿using Storm.Attributes;
using Storm.Collectibles.Currency;
using Storm.Components;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using Storm.LevelMechanics.Platforms;
using Storm.Subsystems.FSM;
using Storm.Subsystems.Transitions;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// The main player script.
  /// </summary>
  /// <remarks>
  /// The player is comprised of states of behavior. See the player's attached animator controller for an idea of this behavior.
  /// </remarks>
  public class PlayerCharacter : MonoBehaviour, IPlayer {
    #region Properties and Component Classes
    /// <summary>
    /// Settings about the player's movement.
    /// </summary>
    public MovementSettings MovementSettings { get; set; }

    /// <summary>
    /// Settings about special effects for the player.
    /// </summary>
    public EffectsSettings EffectsSettings { get; set; }

    /// <summary>
    /// Information about the player's physics. Position, velocity, etc.
    /// </summary>
    public IPhysics Physics { get; set; }

    /// <summary>
    /// Delegate class for collision/distance sensing.
    /// </summary>
    public ICollision CollisionSensor { get; set; }

    /// <summary>
    /// The player's collider.
    /// </summary>
    public Collider2D Collider { get { return playerCollider; } }

    /// <summary>
    /// Delegate class for interacting with stuff.
    /// </summary>
    public IInteractionComponent Interaction { get; set; }

    /// <summary>
    /// The object that the player is currently interacting with.
    /// </summary>
    public Interactible CurrentInteractible { get { return Interaction != null ? Interaction.CurrentInteractible : null; } }

    /// <summary>
    /// Delegate class for the player's inventory.
    /// </summary>
    public IInventory Inventory { get; set;}

    /// <summary>
    /// The player's sprite.
    /// </summary>
  public SpriteRenderer Sprite { get { return sprite; } }

    /// <summary>
    /// The player's state machine. You shouldn't normally need to access this
    /// directly, but this has been left open to use in a pinch.
    /// </summary>
    public FiniteStateMachine FSM { get { return stateMachine; } }

    /// <summary>
    /// A delegate class for handling the player's throwing abilities.
    /// </summary>
    public IThrowing ThrowingComponent { get; set; }

    /// <summary>
    /// The object the player is carrying
    /// </summary>
    /// <value>The object the player should be carrying.</value>
    public Carriable CarriedItem {
      get { return carriedItem; }
      set { carriedItem = value; }
    }

    /// <summary>
    /// Script that handles coyote time for the player.
    /// </summary>
    private CoyoteTimer coyoteTimer;

    /// <summary>
    /// Wrapper class around Unity's static Input class.
    /// </summary>
    private UnityInput unityInput;

    /// <summary>
    /// A UI element for indicating where the player is going to throw.
    /// </summary>
    private ThrowingGuide throwingGuide;

    /// <summary>
    /// Player's behavioral state machine
    /// </summary>
    private FiniteStateMachine stateMachine;

    /// <summary>
    /// Component for handling player death.
    /// </summary>
    private Death death;
    #endregion

    #region Fields
    #region Collision Testing
    /// <summary>
    /// A reference to the player's box collider.
    /// </summary>
    private BoxCollider2D playerCollider;

    /// <summary>
    /// Layer mask that prevents collisions with anything aside from things on the ground layer.
    /// </summary>
    private LayerMask groundLayerMask;
    #endregion

    #region Other Player Information
    /// <summary>
    /// A reference to the player's sprite.
    /// </summary>
    private SpriteRenderer sprite;

    [Header("Debug Information", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// Whether the player is facing left or right.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether the player is facing left or right.")]
    private Facing facing;
    public Facing Facing { get { return facing; } }

    /// <summary>
    /// The center of the player's collider.
    /// </summary>
    public Vector2 Center { get { return playerCollider.bounds.center; } }

    /// <summary>
    /// Whether or not the player is allowed to jump.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is allowed to jump.")]
    private bool canJump = true;

    /// <summary>
    /// Whether or not the player is allowed to move.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is allowed to move.")]
    private bool canMove = true;

    /// <summary>
    /// Whether or not the player is allowed to crouch.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is allowed to crouch.")]
    private bool canCrouch = true;

    /// <summary>
    /// Whether or not the player is wall jumping. This is kept on the
    /// PlayerCharacter class because certain states need to know if the player
    /// is wall jumping, but shouldn't be able to cause a wall jump.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is wall jumping.")]
    private bool isWallJumping;

    /// <summary>
    /// Whether or not the player is standing on a platform.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not the player is standing on a platform.")]
    private bool isOnMovingPlatform;

    /// <summary>
    /// The object the player is carrying at the moment.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("The object the player is carrying at the moment.")]
    private Carriable carriedItem;

    /// <summary>
    /// Whether or not the player can interrupt wall jump trajectory.
    /// </summary>
    private bool canInterruptWallJump;

    #endregion

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      MovementSettings = GetComponent<MovementSettings>();
      EffectsSettings = GetComponent<EffectsSettings>();
      Inventory = GetComponent<PlayerInventory>(); 
      
      sprite = GetComponent<SpriteRenderer>();
      coyoteTimer = gameObject.AddComponent<CoyoteTimer>();
      coyoteTimer.Inject(MovementSettings);

      playerCollider = GetComponent<BoxCollider2D>();

      unityInput = new UnityInput();
      CollisionSensor = new CollisionComponent(playerCollider);

      Physics = gameObject.AddComponent<PhysicsComponent>();
      Interaction = gameObject.AddComponent<InteractionComponent>();

      ThrowingComponent = gameObject.AddComponent<ThrowingComponent>();
      throwingGuide = GetComponentInChildren<ThrowingGuide>();

      death = gameObject.AddComponent<Death>();

      stateMachine = gameObject.AddComponent<FiniteStateMachine>();
      State state = gameObject.AddComponent<Idle>();
      stateMachine.StartMachine(state);
    }

    private void Start() {
      Respawn();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.GetComponent<MovingPlatform>() == null) {
        DisablePlatformMomentum();
        transform.SetParent(null);
      }
    }
    #endregion

    #region Basic Getters/Setters

    /// <summary>
    /// Whether or not jumping is enabled for the player.
    /// </summary>
    /// <returns>True if the player is allowed to jump. False otherwise.</returns>
    public bool CanJump() {
      return canJump;
    }

    /// <summary>
    /// Disable jumping for the player.
    /// </summary>
    public void DisableJump() {
      canJump = false;
    }

    /// <summary>
    /// Enable jumping for the player.
    /// </summary>
    public void EnableJump() {
      canJump = true;
    }

    /// <summary>
    /// Whether or not movement is enabled for the player.
    /// </summary>
    /// <returns>True if the player is allowed to move. False otherwise.</returns>
    public bool CanMove() {
      return canMove;
    }

    /// <summary>
    /// Disable movement for the player.
    /// </summary>
    public void DisableMove() {
      canMove = false;
    }

    /// <summary>
    /// Enable movement for the player.
    /// </summary>
    public void EnableMove() {
      canMove = true;
    }

    /// <summary>
    /// Disable crouching for the player.
    /// </summary>
    public void DisableCrouch() {
      canCrouch = false;
    }

    /// <summary>
    /// Enable crouching for the player.
    /// </summary>
    public void EnableCrouch() {
      canCrouch = true;
    }

    /// <summary>
    /// Whether or not crouching is enabled for the player.
    /// </summary>
    public bool CanCrouch() {
      return canCrouch;
    }

    /// <summary>
    /// Disables player platform momentum.
    /// </summary>
    public void DisablePlatformMomentum() {
      isOnMovingPlatform = false;
    }

    /// <summary>
    /// Enables player platform momentum.
    /// </summary>
    public void EnablePlatformMomentum() {
      isOnMovingPlatform = true;
    }

    /// <summary>
    /// Whether or not platform momentum is enabled.
    /// </summary>
    /// <returns>True if the player is on a platform, false otherwise.</returns>
    public bool IsPlatformMomentumEnabled() {
      return isOnMovingPlatform;
    }

    /// <summary>
    /// Start wall jump muting.
    /// </summary>
    /// <remarks>
    /// Wall jumps have slightly altered physics from normal
    /// jumping to make it slightly harder for the player to return to the wall
    /// they've jumped from. This is known as wall jump muting, and only applies
    /// to the first jump the player makes from a wall.
    /// </remark>
    public void StartWallJumpMuting() {
      isWallJumping = true;
      canInterruptWallJump = false;
    }

    /// <summary>
    /// Stop wall jump muting.
    /// </summary>
    /// <remarks>
    /// Wall jumps have slightly altered physics from normal
    /// jumping to make it slightly harder for the player to return to the wall
    /// they've jumped from. This is known as wall jump muting, and only applies
    /// to the first jump the player makes from a wall.
    /// </remark>
    public void StopWallJumpMuting() {
      isWallJumping = false;
      canInterruptWallJump = false;
    }

    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    public bool IsWallJumping() {
      return isWallJumping;
    }

    /// <summary>
    /// Allow the player to interrupt the horizontal momentum they've gained
    /// from a wall jump.
    /// </summary>
    public void AllowWallJumpInterruption() {
      canInterruptWallJump = true;
    }

    /// <summary>
    /// Whether or not the player can interrupt the horizontal momentum gained
    /// from a wall jump.
    /// </summary>
    /// <returns>True if they can interrupt the wall jump. False otherwise.</returns>
    public bool CanInterruptWallJump() {
      return canInterruptWallJump;
    }
    #endregion

    #region Checking Player State

    /// <summary>
    /// Whether or not the player is rising.
    /// </summary>
    /// <returns>True if the player's vertical velocity is above 0. False otherwise.</returns>
    public bool IsRising() {
      return Physics.Vy > 0;
    }

    /// <summary>
    /// Whether or not the player is falling.
    /// </summary>
    /// <returns>True if the player's vertical velocity is less than or equal to
    /// 0. False otherwise.</returns>
    public bool IsFalling() {
      return Physics.Vy <= 0;
    }

    /// <summary>
    /// Whether or not the player is crouching.
    /// </summary>
    /// <returns>True if the player is crouching or starting/ending a crouch,
    /// false otherwise.</returns>
    public bool IsCrouching() {
      return stateMachine.IsInState<CrouchStart>() ||
        stateMachine.IsInState<Crouching>() ||
        stateMachine.IsInState<CrouchEnd>();
    }

    /// <summary>
    /// Whether or not the player is crawling.
    /// </summary>
    public bool IsCrawling() {
      return stateMachine.IsInState<Crawling>();
    }

    /// <summary>
    /// Whether or not the player is diving into a crawl.
    /// </summary>
    public bool IsDiving() {
      return stateMachine.IsInState<Dive>();
    }

    /// <summary>
    /// Whether or not the player is wall running or wall sliding.
    /// </summary>
    public bool IsInWallAction() {
      return stateMachine.IsInState<WallRun>() || stateMachine.IsInState<WallSlide>();
    }
    #endregion

    #region Player Facing
    /// <summary>
    /// Sets the direction that the player is facing.
    /// </summary>
    /// <param name="facing">The direction enum</param>
    public void SetFacing(Facing facing) {
      if (facing != Facing.None && facing != this.facing) {
        this.facing = facing;
        
        Vector3 scale = this.throwingGuide.transform.localScale;
        this.throwingGuide.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
      }

      if (facing == Facing.Left) {
        transform.localScale = new Vector3(-1, 1, 1);
        if (Interaction.CurrentIndicator != null && 
            Interaction.CurrentIndicator.transform.parent == transform) {
          Interaction.CurrentIndicator.transform.localScale = new Vector3(-1, 1, 1);
        }
      } else if (facing == Facing.Right) {
        transform.localScale = new Vector3(1, 1, 1);
        if (Interaction.CurrentIndicator != null &&
            Interaction.CurrentIndicator.transform.parent == transform) {
          Interaction.CurrentIndicator.transform.localScale = new Vector3(1, 1, 1);
        }
      }

      if (CarriedItem != null) {
        CarriedItem.transform.localScale = Vector3.one;
      }
    }
    #endregion

    #region Collision Checking Delegation

    /// <summary>
    /// How far the player is from the ground.
    /// </summary>
    /// <returns>The distance between the player's feet and the closest piece of ground.</returns>
    public float DistanceToGround() => CollisionSensor.DistanceToGround(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    public float DistanceToLeftWall() => CollisionSensor.DistanceToLeftWall(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    public float DistanceToRightWall() => CollisionSensor.DistanceToRightWall(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the player is from the closest wall.
    /// </summary>
    /// <returns>The distance between the player and the closest wall.</returns>
    public float DistanceToWall() => CollisionSensor.DistanceToWall(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// How far the object is from the closest ceiling.
    /// </summary>
    /// <returns>The distance between the object and the closest ceiling.</returns>
    public float DistanceToCeiling() => CollisionSensor.DistanceToCeiling(playerCollider.bounds.center, playerCollider.bounds.extents);

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    public bool IsTouchingLeftWall() => CollisionSensor.IsTouchingLeftWall(playerCollider.bounds.center, playerCollider.bounds.size);

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    public bool IsTouchingRightWall() => CollisionSensor.IsTouchingRightWall(playerCollider.bounds.center, playerCollider.bounds.size);

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    public bool IsTouchingGround() => CollisionSensor.IsTouchingGround(playerCollider.bounds.center, playerCollider.bounds.size);
    
    
    /// <summary>
    /// Whether or not the object is touching the ceiling.
    /// </summary>
    public bool IsTouchingCeiling() => CollisionSensor.IsTouchingCeiling(playerCollider.bounds.center, playerCollider.bounds.size);
    
    /// <summary>
    /// Whether or not the player will fit in a position one space below where they
    /// currently are.
    /// </summary>
    /// <returns>Returns true if the player would fit in the space directly below
    /// their feet.</returns>
    public bool FitsDown(out Collider2D[] hits) => CollisionSensor.FitsDown(playerCollider.bounds.center, playerCollider.bounds.size, out hits);

    /// <summary>
    /// Whether or not a box will fit in a position one space above where it
    /// currently is.
    /// </summary>
    /// <returns>Returns true if the box would fit in the space directly above
    /// it's top.</returns>
    public bool FitsUp(out Collider2D[] hits) => CollisionSensor.FitsUp(playerCollider.bounds.center, playerCollider.bounds.size, out hits);

    /// <summary>
    /// Whether or not a box will fit in a position one space to the left of where it
    /// currently is.
    /// </summary>
    /// <returns>Returns true if the box would fit in the space directly to its left.</returns>
    public bool FitsLeft(out Collider2D[] hits) => CollisionSensor.FitsLeft(playerCollider.bounds.center, playerCollider.bounds.size, out hits);

    /// <summary>
    /// Whether or not a box will fit in a position one space to the right of where it
    /// currently is.
    /// </summary>
    /// <returns>Returns true if the box would fit in the space directly to its right.</returns>
    public bool FitsRight(out Collider2D[] hits) => CollisionSensor.FitsRight(playerCollider.bounds.center, playerCollider.bounds.size, out hits);
    
    /// <summary>
    /// Whether or not a box will fit in a position one space to the right of where it
    /// currently is.
    /// </summary>
    /// <param name="direction">The direction to check</param>
    /// <returns>Returns true if the box would fit in the space directly to its right.</returns>
    public bool FitsInDirection(Vector2 direction, out Collider2D[] hits) => CollisionSensor.FitsInDirection(playerCollider.bounds.center, playerCollider.bounds.size, direction, out hits);
    

    /// <summary>
    /// Whether or not a collision can be considered a valid "ground" collision.
    /// </summary>
    /// <param name="collider">The collider of the object that might be hitting
    /// the player.</param>
    /// <param name="hitNormal">The normal for the collision.</param>
    /// <param name="checkNormal">The normal expected.</param>
    /// <returns>True if this is a valid "ground" hit. False otherwise.</returns>
    public bool IsHitBy(Collider2D collider, Vector2? hitNormal = null, Vector2? checkNormal = null) => CollisionSensor.IsHit(collider, hitNormal, checkNormal);
    #endregion

    #region Input Checking Delegation

    /// <summary>
    /// Checks if the player pressed the jump button.
    /// </summary>
    /// <returns>True if the player pressed the jump button.</returns>
    public bool PressedJump() => unityInput.GetButtonDown("Jump") && CanJump();

    /// <summary>
    /// Checks if the player is holding the jump button.
    /// </summary>
    /// <returns>True if the player is holding the jump button.</returns>
    public bool HoldingJump() => unityInput.GetButton("Jump") && CanJump();

    /// <summary>
    /// Checks whether or not the player is trying to move horizontally, and whether or not they're allowed to.
    /// </summary>
    /// <returns>True if the player should move.</returns>
    public bool TryingToMove() => CanMove() && unityInput.GetHorizontalInput() != 0;

    /// <summary>
    /// Checks if the player has pressed the up button.
    /// </summary>
    /// <returns>True if the player pressed up in the current frame.</returns>
    public bool PressedUp() => unityInput.GetButtonDown("Up") && CanCrouch();

    /// <summary>
    /// Checks if the player is holding down the up button.
    /// </summary>
    /// <returns>True if the player is holding down the up button</returns>
    public bool HoldingUp() => unityInput.GetButton("Up") && CanCrouch();

    /// <summary>
    /// Checks if the player has released the up button.
    /// </summary>
    /// <returns>True if the player has released up.</returns>
    public bool ReleasedUp() => unityInput.GetButtonUp("Up");


    /// <summary>
    /// Checks if the player has pressed the down button.
    /// </summary>
    /// <returns>True if the player pressed down in the current frame.</returns>
    public bool PressedDown() => unityInput.GetButtonDown("Down") && CanCrouch();

    /// <summary>
    /// Checks if the player is holding down the down button.
    /// </summary>
    /// <returns>True if the player is holding down the down button</returns>
    public bool HoldingDown() => unityInput.GetButton("Down") && CanCrouch();

    /// <summary>
    /// Checks if the player has released the down button.
    /// </summary>
    /// <returns>True if the player has released down.</returns>
    public bool ReleasedDown() => unityInput.GetButtonUp("Down");

    /// <summary>
    /// Gets the horizontal input axis for the player.
    /// </summary>
    /// <returns>The horizontal input of the player from -1 (left) to 1 (right)</returns>
    public float GetHorizontalInput() => unityInput.GetHorizontalInput();

    /// <summary>
    /// Whether or not the player has pressed the action button.
    /// </summary>
    /// <returns>True if the player has press the action button. False otherwise.</returns>
    public bool PressedAction() => unityInput.GetButtonDown("Action");

    /// <summary>
    /// Whether or not the player is holding the action button down.
    /// </summary>
    /// <returns>True if the player is holding action button down. False otherwise.</returns>
    public bool HoldingAction() => unityInput.GetButton("Action");

    /// <summary>
    /// Whether or not the player has released the action button.
    /// </summary>
    /// <returns>True if the palyer has released the action button. False otherwise.</returns>
    public bool ReleasedAction() => unityInput.GetButtonUp("Action");

    public bool PressedAltAction() => unityInput.GetButtonDown("AltAction");

    public bool HoldingAltAction() => unityInput.GetButton("AltAction");

    public bool ReleasedAltAction() => unityInput.GetButtonUp("AltAction");

    /// <summary>
    /// Gets the mouse position on the screen.
    /// </summary>
    public Vector3 GetMouseScreenPosition() => unityInput.GetMouseScreenPosition();
    
    /// <summary>
    /// Gets the mouse position within the world.
    /// </summary>
    public Vector3 GetMouseWorldPosition() => unityInput.GetMouseWorldPosition();

    #endregion

    #region Coyote Time Delegation
    /// <summary>
    /// Starts coyote time for the player. After leaving a ledge, the player will still have a fraction of a
    /// second to input a jump.
    /// </summary>
    public void StartCoyoteTime() => coyoteTimer.StartCoyoteTime();


    /// <summary>
    /// Whether or not the player still has time to input a jump after leaving a
    /// ledge.
    /// </summary>
    /// <returns>True if the player still has time to jump. False otherwise.</returns>
    public bool InCoyoteTime() => coyoteTimer.InCoyoteTime();


    /// <summary>
    /// Use up the remaining coyote time. This should be called after the player
    /// performs a jump just after walking off a ledge.
    /// </summary>
    public void UseCoyoteTime() => coyoteTimer.UseCoyoteTime();

    #endregion

    #region Interaction Delegation
    /// <summary>
    /// Try to interact with the closest interactive object.
    /// </summary>
    public void Interact() => Interaction.Interact();

    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to add.</param>
    public void AddInteractible(Interactible interactible) => Interaction.AddInteractible(interactible);
    
    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to remove.</param>
    public void RemoveInteractible(Interactible interactible) => Interaction.RemoveInteractible(interactible);

    /// <summary>
    /// Register an interaction indicator with the player.
    /// </summary>
    /// <param name="indicator">The indicator to register</param>
    public void RegisterIndicator(Indicator indicator) => Interaction.RegisterIndicator(indicator);
    
    /// <summary>
    /// Clear the indicator cache. Used between levels.
    /// </summary>
    public void ClearIndicators() => Interaction.ClearIndicators();

    #endregion

    #region Death/Respawn Delegation

    /// <summary>
    /// Kill the player.
    /// </summary>
    public void Die() => death.Die();

    /// <summary>
    /// Respawn the player.
    /// </summary>
    public void Respawn() => death.Respawn();
    
    /// <summary>
    /// Whether or not the player is currently dead.
    /// </summary>
    public bool IsDead() => death.IsDead();
    #endregion


    #region Throwing Delegation

    /// <summary>
    /// Throw the given item
    /// </summary>
    /// <param name="carriable">The item to throw.</param>
    public void Throw(Carriable carriable) => ThrowingComponent.Throw(carriable);

    /// <summary>
    /// Drop the given item.
    /// </summary>
    /// <param name="carriable">The item to drop.</param>
    public void Drop(Carriable carriable) => ThrowingComponent.Drop(carriable);


    /// <summary>
    /// The direction the player would throw an item they may be holding.
    /// </summary>
    /// <param name="normalized">Whether or not to normalize the direction
    /// (default: true)</param>
    public Vector2 GetThrowingDirection(bool normalized = true) => ThrowingComponent.GetThrowingDirection(normalized);
    
    /// <summary>
    /// The position that the player's throw would start.
    /// </summary>
    public Vector2 GetThrowingPosition() => ThrowingComponent.GetThrowingPosition();
    #endregion

    #region Inventory Management
    /// <summary>
    /// Add currency of a particular type to the player's total.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <param name="amount">The amount to add.</param>
    /// <seealso cref="IInventory.AddCurrency" />
    public void AddCurrency(string name, float amount) => Inventory.AddCurrency(name, amount);

    /// <summary>
    /// Spend some currency of a particular type.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <param name="amount">The amount to spend.</param>
    /// <returns>
    /// True if the the player had enough currency to spend. 
    /// Otherwise, returns false and no currency is removed.
    /// </returns>
    /// <seealso cref="IInventory.SpendCurrency" />
    public bool SpendCurrency(string name, float amount) => Inventory.SpendCurrency(name, amount);

    /// <summary>
    /// Get the total for a particular currency.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <returns>The amount of currency the player has of that type.</returns>
    /// <seealso cref="IInventory.GetCurrencyTotal" />
    public float GetCurrencyTotal(string name) => Inventory.GetCurrencyTotal(name);
    
    /// <summary>
    /// Whether or not the player has a particular currency in their inventory.
    /// </summary>
    /// <param name="name">The name of the currency.</param>
    /// <returns>True if the player has the currency in they inventory. False otherwise.</returns>
    public bool ContainsCurrency(string name) => Inventory.ContainsCurrency(name);
    
    /// <summary>
    /// Clear out the player's inventory.
    /// </summary>
    public void ClearInventory() => Inventory.Clear();
    #endregion
  }
}
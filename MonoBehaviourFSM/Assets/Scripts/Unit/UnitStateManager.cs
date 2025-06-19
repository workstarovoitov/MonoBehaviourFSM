using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the state machine and input for a unit, including state transitions and context data.
/// </summary>
public class UnitStateManager : MonoBehaviour
{
    // Input properties
    public Vector2 MoveInput { get; set; } = Vector2.zero;
    public bool IsRunPerformed { get; set; } = false;
    public bool IsCrouchPerformed { get; set; } = false;
    public bool IsJumpPerformed { get; set; } = false;
    public bool IsAttackPerformed { get; set; } = false;

    public event System.Action OnMoveInputUpdate;

    // State management
    private Dictionary<UNITSTATE, UnitStateBase> stateMap;
    public UnitStateBase CurrentStateBase { get; private set; }

    [SerializeField]
    private UNITSTATE currentState = UNITSTATE.IDLE;
    public UNITSTATE CurrentState => currentState;

    // Environment and context properties
    private int currentJumpCount = 0;
    public int CurrentJumpCount 
    {
        get => currentJumpCount;
        set => currentJumpCount = value;
    }

    private bool isGrounded = true;
    public bool IsGrounded => isGrounded;

    private bool onPlatform = false;
    public bool OnPlatform
    {
        get => onPlatform;
        set => onPlatform = value;
    }

    private string surfaceMaterial = null;
    public string SurfaceMaterial
    {
        get => surfaceMaterial;
        set => surfaceMaterial = value;
    }

    private int defaultLayer;
    public int DefaultLayer
    {
        get => defaultLayer;
        set => defaultLayer = value;
    }

    private string defaultSortingLayer;
    public string DefaultSortingLayer
    {
        get => defaultSortingLayer;
        set => defaultSortingLayer = value;
    }

    private UnitMain uMain;
    private bool skipUpdateThisFrame = false;

    private void Awake()
    {
        stateMap = new Dictionary<UNITSTATE, UnitStateBase>();
        foreach (var state in GetComponentsInChildren<UnitStateBase>())
        {
            state.enabled = false;
            stateMap[state.StateType] = state;
        }
    }

    /// <summary>
    /// Initializes the state manager with a reference to the main unit.
    /// </summary>
    public void Initialize(UnitMain unitMain)
    {
        if (unitMain == null)
        {
            Debug.LogError("UnitMain reference is null in UnitStateManager.");
            enabled = false;
            return;
        }

        uMain = unitMain;
        enabled = true;

        if (stateMap.TryGetValue(currentState, out var newState))
        {
            CurrentStateBase = newState;
            CurrentStateBase.Enter(uMain);
        }
        else
        {
            Debug.LogWarning($"State {currentState} not found on {gameObject.name}");
        }
    }

    private void Update()
    {
        if (uMain == null)
        {
            enabled = false;
            return;
        }

        if (skipUpdateThisFrame)
        {
            skipUpdateThisFrame = false;
            return; // Skip this frame's Update logic
        }

        uMain.uDirection.UpdateDirection();

        if (currentState != UNITSTATE.JUMPDOWN)
            uMain.uCollisions.TryToRestorePlatformsCollision();
        if (currentState != UNITSTATE.FALL)
            uMain.uCollisions.RestoreLedge();

        isGrounded = uMain.uCollisions.IsGrounded();
        CurrentStateBase?.StateUpdate();
    }

    private void FixedUpdate()
    {
        if (uMain == null)
        {
            enabled = false;
            return;
        }

        CurrentStateBase?.StateFixedUpdate();
    }

    /// <summary>
    /// Switches to a new state if the transition is valid.
    /// </summary>
    public void SwitchState(UNITSTATE newStateType)
    {
        if (stateMap.TryGetValue(newStateType, out var newState))
        {
            if (newState.IsAcceptableState(CurrentStateBase.StateType))
            {
                CurrentStateBase.Exit();
                currentState = newStateType;
                CurrentStateBase = newState;
                CurrentStateBase.Enter(uMain);
                Debug.Log($"Switched state to {newStateType} on {gameObject.name}");
                skipUpdateThisFrame = true; // Skip next Update
            }
        }
        else
        {
            Debug.LogWarning($"State {newStateType} not found on {gameObject.name}");
        }
    }

    /// <summary>
    /// Resets the state to IDLE or FALL depending on grounded status, unless in DEATH state.
    /// </summary>
    public void ResetState()
    {
        if (currentState == UNITSTATE.DEATH)
            return;

        if (isGrounded)
            SwitchState(UNITSTATE.IDLE);
        else
            SwitchState(UNITSTATE.FALL);
    }

    /// <summary>
    /// Updates the direction based on current input and notifies listeners.
    /// </summary>
    public void SetActualDirection()
    {
        OnMoveInputUpdate?.Invoke();
        uMain.uDirection.UpdateDirection();
    }
}

/// <summary>
/// Enumerates all possible unit states.
/// </summary>
public enum UNITSTATE
{
    IDLE,
    IDLECROUCH,
    WALK,
    CROUCH,
    RUN,
    DASH,
    JUMP,
    JUMPHIGH,
    JUMPDOWN,
    JUMPSECOND,
    JUMPWALL,
    FALL,
    LAND,
    GRABWALL,
    CLIMBWALL,
    GRABLEDGE,
    GRABPLATFORM,
    CLIMBLEDGE,
    CLIMBSTAIRS,
    CLIMBUP,
    USE,
    ATTACK,
    HURT,
    STUN,
    FREEZED,
    DEATH,
    SLEEP,
    HIDE,
    APPEAR,
    PICKUPITEM,
}
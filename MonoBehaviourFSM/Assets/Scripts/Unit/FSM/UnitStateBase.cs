using UnityEngine;
using System.Collections.Generic;

public abstract class UnitStateBase : MonoBehaviour
{
    internal UnitMain uMain;
    public abstract UNITSTATE StateType { get; }

    [SerializeField]
    internal List<UNITSTATE> forbiddenStates;
    
    protected virtual IMovementStrategy MovementStrategy { get; }
    [SerializeField]
    protected MovementContext movementSettings;

    internal MovementContext movementContext;
    
    public bool IsAcceptableState(UNITSTATE state)
    {
        if (forbiddenStates == null || forbiddenStates.Count == 0) return true;
        if (forbiddenStates.Contains(state)) return false;
        return true;
    }

    public virtual void Enter(UnitMain unitMain) 
    { 
        enabled = true;
        if (uMain == null) uMain = unitMain;
        movementContext = new MovementContext(movementSettings);
    }
    
    public virtual void Exit() { enabled = false; }
    public virtual void StateUpdate() { }
    public virtual void StateFixedUpdate() 
    {
        MovementStrategy?.ApplyMovement(uMain, movementContext);
    }

    // Input hooks
    public virtual void OnMove(Vector2 direction) { }
    public virtual void OnRun(bool isRunning) { }
    public virtual void OnCrouch(bool isCrouching) { }
    public virtual void OnJump() { }
    public virtual void OnJumpCanceled() { }
    public virtual void OnAttack() { }
}

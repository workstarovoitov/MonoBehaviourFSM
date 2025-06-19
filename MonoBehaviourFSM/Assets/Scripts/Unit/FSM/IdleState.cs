using UnityEngine;

public class IdleState : UnitStateBase
{
    public override UNITSTATE StateType => UNITSTATE.IDLE;
   
    protected override IMovementStrategy MovementStrategy { get; } = new DefaultMovementStrategy();

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        uMain.uAnimator.SetAnimatorTrigger("Idle");
        uMain.uAnimator.SetAnimatorBool("IsCrouching", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        
        // Check if the unit is grounded and not moving
        if (!uMain.uState.IsGrounded)
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
            return;
        }
    }

    public override void OnCrouch(bool isCrouching) 
    { 
        if (isCrouching)
        {
            uMain.uState.SwitchState(UNITSTATE.IDLECROUCH);
        }
    }

    public override void OnMove(Vector2 direction)
    {
        if (direction.x != 0)
        {
            if (uMain.uState.IsRunPerformed) uMain.uState.SwitchState(UNITSTATE.RUN);
            else uMain.uState.SwitchState(UNITSTATE.WALK);
        }
    }

    public override void OnJump()
    {
        if (uMain.uState.MoveInput.y == -1 && uMain.uState.OnPlatform)
        {
            uMain.uState.SwitchState(UNITSTATE.JUMPDOWN);
        }
        else
        {
            uMain.uState.SwitchState(UNITSTATE.JUMP);
        }
    }
}

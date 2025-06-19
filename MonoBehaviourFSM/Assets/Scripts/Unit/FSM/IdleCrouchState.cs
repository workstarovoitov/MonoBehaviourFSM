using UnityEngine;

public class IdleCrouchState : UnitStateBase
{
    public override UNITSTATE StateType => UNITSTATE.IDLECROUCH;

    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        uMain.uAnimator.SetAnimatorTrigger("Idle");
        uMain.uAnimator.SetAnimatorBool("IsCrouching", true);
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
        
        if (!uMain.uState.IsCrouchPerformed && !uMain.uCollisions.WallOnTop())
        {
            uMain.uState.SwitchState(UNITSTATE.IDLE);
            return;
        }

        if (uMain.uState.MoveInput.x != 0 && !uMain.uCollisions.CliffInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.CROUCH);
            return;
        }
    }

    public override void OnMove(Vector2 direction)
    {
        if (direction.x != 0 && !uMain.uCollisions.CliffInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.CROUCH);
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
            uMain.uState.SwitchState(UNITSTATE.JUMPHIGH);
        }
    }
}

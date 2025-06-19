using UnityEngine;

public class CrouchState : UnitStateBase
{
    public override UNITSTATE StateType => UNITSTATE.CROUCH;
    protected override IMovementStrategy MovementStrategy { get; } = new DefaultMovementStrategy();

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        uMain.uAnimator.SetAnimatorTrigger("Walk");
        uMain.uAnimator.SetAnimatorBool("IsCrouching", true);
        uMain.uAnimator.SetAnimatorBool("IsRunning", false);
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
        if (uMain.uCollisions.WallInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.IDLECROUCH);
            return;
        }

        if (uMain.uCollisions.CliffInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.IDLECROUCH);
            return;
        }

        // Check if the unit is grounded and not moving
        if (!uMain.uState.IsGrounded)
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
            return;
        }
      
        if (!uMain.uState.IsCrouchPerformed && !uMain.uCollisions.WallOnTop())
        {
            if (uMain.uState.IsRunPerformed) uMain.uState.SwitchState(UNITSTATE.RUN);
            else uMain.uState.SwitchState(UNITSTATE.WALK);
            return;
        }
    }

    public override void OnMove(Vector2 direction)
    {
        if (direction.x == 0)
        {
            uMain.uState.SwitchState(UNITSTATE.IDLECROUCH);
        }
        else if (uMain.uCollisions.WallInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.IDLECROUCH);
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

using UnityEngine;

public class WalkState : UnitStateBase
{
    public override UNITSTATE StateType => UNITSTATE.WALK;

    protected override IMovementStrategy MovementStrategy { get; } = new DefaultMovementStrategy();

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        uMain.uAnimator.SetAnimatorTrigger("Walk");
        uMain.uAnimator.SetAnimatorBool("IsCrouching", false);
        uMain.uAnimator.SetAnimatorBool("IsRunning", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (uMain.uCollisions.WallInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.IDLE);
            return;
        }

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
            uMain.uState.SwitchState(UNITSTATE.CROUCH);
        }
    }

    public override void OnRun(bool isRunning)
    {
        if (isRunning)
        {
            uMain.uState.SwitchState(UNITSTATE.RUN);
        }
    }

    public override void OnMove(Vector2 direction)
    {
        if (direction.x == 0)
        {
            uMain.uState.SwitchState(UNITSTATE.IDLE);
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

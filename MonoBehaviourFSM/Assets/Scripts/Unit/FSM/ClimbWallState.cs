using UnityEngine;

public class ClimbWallState : UnitStateBase
{
    [SerializeField] private float climbUpSpeed = 2f;
    [SerializeField] private float climbDownSpeed = 0.5f;

    public override UNITSTATE StateType => UNITSTATE.CLIMBWALL;
    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        uMain.uAnimator.SetAnimatorTrigger("ClimbWall");

        SetCustomVelocity();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (TryGrabLedge()) return;
        if (TryLand()) return;
        if (TryFall()) return;
    }

    private bool TryLand()
    {
        if (uMain.uState.IsGrounded && uMain.rb.linearVelocity.y <= 0)
        {
            uMain.uState.SwitchState(UNITSTATE.LAND);
            return true;
        }
        return false;
    }

    private bool TryFall()
    {
        if (!uMain.uCollisions.WallForGrabInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
            return true;
        }
        return false;
    }

    private bool TryGrabLedge()
    {
        if (uMain.uCollisions.IsLedgeInFront())
        {
            uMain.uState.SwitchState(UNITSTATE.GRABLEDGE);
            return true;
        }
        return false;
    }

    public override void OnMove(Vector2 direction)
    {
        SetCustomVelocity();
        if (direction.y == 0)
        {
            uMain.uState.SwitchState(UNITSTATE.GRABWALL);
        }
    }

    private void SetCustomVelocity()
    {
        if (uMain.uState.MoveInput.y > 0)
        {
            movementContext.MaxSpeed = new Vector2(0f, climbUpSpeed);
        }
        else if (uMain.uState.MoveInput.y < 0)
        {
            movementContext.MaxSpeed = new Vector2(0f, -climbDownSpeed);
        }
        else
        {
            movementContext.MaxSpeed = Vector2.zero;
        }
    }

    public override void OnJump()
    {
        uMain.uState.SwitchState(UNITSTATE.JUMPWALL);
    }
}
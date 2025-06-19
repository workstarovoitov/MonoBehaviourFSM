using UnityEngine;

public class FallState : UnitStateBase
{
    // Threshold for "near zero" velocity
    [SerializeField] private float apexThreshold = 1f;
    [SerializeField] private float platformGrabThreshold = 10f;

    public override UNITSTATE StateType => UNITSTATE.FALL;

    protected override IMovementStrategy MovementStrategy { get; } = new DefaultMovementStrategy();

    private bool isFalling;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);

        movementContext.MaxSpeed.x = Mathf.Max(Mathf.Abs(uMain.rb.linearVelocity.x), movementContext.MaxSpeed.x);

        uMain.uAnimator.SetAnimatorTrigger("Fall");
        if (uMain.rb.linearVelocity.y > apexThreshold) // Check if the unit is falling down
        {
            uMain.uAnimator.SetAnimatorBool("IsFalling", false);
            isFalling = false;
        } 
        else
        {
            uMain.uAnimator.SetAnimatorBool("IsFalling", true);
            isFalling = true;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        ProcessFallingVerticalDirection();
        if (TryGrabLedge()) return;
        if (TryGrabPlatform()) return;
        if (TryGrabWall()) return;
        if (TryLand()) return;
    }

    private void ProcessFallingVerticalDirection()
    {
        float yVel = Mathf.Abs(uMain.rb.linearVelocity.y);

        if (isFalling && uMain.rb.linearVelocity.y > -apexThreshold)
        {
            isFalling = !isFalling; // Toggle the falling state
            uMain.uAnimator.SetAnimatorBool("IsFalling", false);
        }
        if (!isFalling && uMain.rb.linearVelocity.y < apexThreshold)
        {
            isFalling = !isFalling; // Toggle the falling state
            uMain.uAnimator.SetAnimatorBool("IsFalling", true);
        }
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
    
    private bool TryGrabLedge()
    {
        // Replace 'IsLedgeInFront' with your actual ledge detection method
        if (uMain.uCollisions.IsLedgeInFront())
        {
            uMain.uState.SwitchState(UNITSTATE.GRABLEDGE);
            return true;
        }
        return false;
    }

    private bool TryGrabPlatform()
    {
        // Replace 'IsLedgeInFront' with your actual ledge detection method
        if (uMain.uCollisions.IsPlatformInFront() && uMain.rb.linearVelocity.y > apexThreshold && uMain.rb.linearVelocity.y < platformGrabThreshold)
        {
            uMain.uState.SwitchState(UNITSTATE.GRABPLATFORM);
            return true;
        }
        return false;
    }

    private bool TryGrabWall()
    {
        if (uMain.uCollisions.WallForGrabInFrontX() && uMain.uState.MoveInput.x != 0)
        {
            uMain.uState.SwitchState(UNITSTATE.GRABWALL);
            return true;
        }
        return false;
    }

    public override void OnJump()
    {
        if (uMain.uCollisions.WallInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.JUMPWALL);
        }
        else if (uMain.uState.CurrentJumpCount < uMain.MaxJumps) 
        {
            uMain.uState.SwitchState(UNITSTATE.JUMPSECOND);
        }
    }
}

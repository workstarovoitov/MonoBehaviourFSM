using UnityEngine;

public class JumpDownState : UnitStateBase
{
    [SerializeField] private int jumpIterationsNum = 2;

    // Removed the setter from the overridden property to match the base class definition
    private IMovementStrategy movementStrategy = new ConstantSpeedMovementStrategy();
    protected override IMovementStrategy MovementStrategy
    {
        get => movementStrategy;
    }

    private bool readyToJump = false;
    private GameObject platform = null;
    private int currentJumpIteration = 0;

    public override UNITSTATE StateType => UNITSTATE.JUMPDOWN;

    public override void Enter(UnitMain unitMain)
    {
        readyToJump = false;
        currentJumpIteration = 0;

        base.Enter(unitMain);

        uMain.uAnimationProxy.OnStartJump += StartJumping;

        uMain.uAnimator.SetAnimatorTrigger("JumpDown");
        
        platform = uMain.uCollisions.Platform;
        movementContext.MaxSpeed = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
        uMain.uAnimationProxy.OnStartJump -= StartJumping;
    }

    public override void StateFixedUpdate()
    {
        if (!readyToJump)
        {
            MovementStrategy?.ApplyMovement(uMain, movementContext);
            return;
        }

        if (currentJumpIteration < jumpIterationsNum)
        {
            MovementStrategy?.ApplyMovement(uMain, movementContext);
            currentJumpIteration++;
            return;
        }

        if (uMain.uState.IsGrounded)
        {
            uMain.uState.SwitchState(UNITSTATE.LAND);
        }
        else
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
        }
    }

    private void StartJumping()
    {
        uMain.uCollisions.IgnorePlatformCollision(platform);

        movementContext.MaxSpeed = new Vector2(0, -movementSettings.MaxSpeed.y);

        readyToJump = true;
    }
}

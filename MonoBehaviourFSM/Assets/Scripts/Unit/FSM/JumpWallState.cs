using UnityEngine;

public class JumpWallState : UnitStateBase
{
    [SerializeField] private int jumpIterationsNum = 3;

    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();

    private int currentJumpIteration = 0;
    private bool readyToJump = false;

    public override UNITSTATE StateType => UNITSTATE.JUMPWALL;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);

        currentJumpIteration = 0;
        readyToJump = false;
        uMain.uState.MoveInput = Vector2.zero; // Reset move input to prevent unwanted movement

        // Optionally, subscribe to animation event if needed
        uMain.uAnimator.SetAnimatorTrigger("JumpWall");
        uMain.uAnimationProxy.OnStartJump += StartJumping;
        
        movementContext.MaxSpeed = Vector2.zero;

        uMain.uState.CurrentJumpCount = 1;
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
        }
        else
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
        }
    }

    public void StartJumping()
    {
        // Determine wall jump direction: away from current facing
        uMain.uDirection.ReverseDirection();
        movementContext.MaxSpeed = new Vector2(movementSettings.MaxSpeed.x * (float)uMain.uDirection.CurrentDirection, movementSettings.MaxSpeed.y);
        readyToJump = true;
    }
}

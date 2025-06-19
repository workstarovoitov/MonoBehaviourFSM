using UnityEngine;

public class JumpSecondState : UnitStateBase
{
    [SerializeField] private int jumpIterationsNum = 3;

    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();

    private int currentJumpIteration = 0;
    private bool readyToJump = false;

    public override UNITSTATE StateType => UNITSTATE.JUMPSECOND;

    public override void Enter(UnitMain unitMain)
    {
        currentJumpIteration = 0;
        readyToJump = false;

        base.Enter(unitMain);
        uMain.uAnimationProxy.OnStartJump += StartJumping;

        uMain.uAnimator.SetAnimatorTrigger("JumpSecond");
       
        movementContext.MaxSpeed = Vector2.zero;
        
        uMain.uState.CurrentJumpCount++;
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

    private void StartJumping()
    {
        movementContext.MaxSpeed = new Vector2((float)uMain.uDirection.CurrentDirection * movementSettings.MaxSpeed.x, movementSettings.MaxSpeed.y);
        readyToJump = true;
    }
}
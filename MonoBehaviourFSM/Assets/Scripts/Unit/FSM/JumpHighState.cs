using UnityEngine;

public class JumpHighState : UnitStateBase
{
    [SerializeField] private int jumpIterationsNum = 10;
    [SerializeField] private int minJumpIterationsNum = 2;
    [SerializeField] private float speedDecreaseFactor = 1f;
    protected override IMovementStrategy MovementStrategy { get; } = new JumpMovementStrategy();


    private int currentJumpIteration = 0;
    private bool jumpButtonHold = false;
    private bool readyToJump = false;

    public override UNITSTATE StateType => UNITSTATE.JUMPHIGH;

    public override void Enter(UnitMain unitMain)
    {
        currentJumpIteration = 0;
        jumpButtonHold = true;
        readyToJump = false;

        base.Enter(unitMain);
        uMain.uAnimationProxy.OnStartJump += StartJumping;
        movementContext.MaxSpeed.x = Mathf.Max(Mathf.Abs(uMain.rb.linearVelocity.x), movementContext.MaxSpeed.x);

        uMain.uAnimator.SetAnimatorTrigger("Jump");

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
            return;

        if (currentJumpIteration < jumpIterationsNum && (jumpButtonHold || currentJumpIteration < minJumpIterationsNum))
        {
            MovementStrategy?.ApplyMovement(uMain, movementContext);
            currentJumpIteration++;
            if (currentJumpIteration >= minJumpIterationsNum)
            {
                movementContext.MaxSpeed.y -= speedDecreaseFactor; // Decrease vertical speed over time
                if (movementContext.MaxSpeed.y < 0)
                {
                    movementContext.MaxSpeed.y = 0; // Prevent negative speed
                }
            }
        }
        else
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
        }
    }

    public override void OnJumpCanceled()
    {
        jumpButtonHold = false;
    }

    public void StartJumping()
    {
        readyToJump = true;
    }
}
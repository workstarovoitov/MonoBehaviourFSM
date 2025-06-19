using UnityEngine;

public class JumpState : UnitStateBase
{
    [SerializeField] private int jumpIterationsNum = 5;
    [SerializeField] private int minJumpIterationsNum = 1;

    protected override IMovementStrategy MovementStrategy { get; } = new JumpMovementStrategy();


    private int currentJumpIteration = 0;
    private bool jumpButtonHold = false;
    private bool readyToJump = false;

    public override UNITSTATE StateType => UNITSTATE.JUMP;

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

    private void StartJumping()
    {
        readyToJump = true;
    }
}

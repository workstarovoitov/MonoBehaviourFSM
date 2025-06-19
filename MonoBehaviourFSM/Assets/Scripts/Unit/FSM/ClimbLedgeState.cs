using UnityEngine;

public class ClimbLedgeState : UnitStateBase
{
    [SerializeField] private Vector2 climbOffset;

    public override UNITSTATE StateType => UNITSTATE.CLIMBLEDGE;

    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();
    private EndAnimationBehaviour endAnimBehaviour;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);

        // Find the EndAnimationBehaviour attached to the relevant state
        var animator = uMain.uAnimator.Animator; // Adjust as needed for your setup
        foreach (var behaviour in animator.GetBehaviours<EndAnimationBehaviour>())
        {
            if (behaviour.stateName == "ClimbLedge")
            {
                endAnimBehaviour = behaviour;
                break;
            }
        }

        if (endAnimBehaviour != null)
        {
            endAnimBehaviour.OnEndAnimation = (anim, stateInfo, layerIndex) => FinishClimbing();
        }

        uMain.uAnimator.SetAnimatorTrigger("ClimbLedge");
    }

    public override void Exit()
    {
        if (endAnimBehaviour != null)
        {
            endAnimBehaviour.OnEndAnimation = null;
        }
        base.Exit();
    }

    private void FinishClimbing()
    {
        float directionSign = Mathf.Sign((float)uMain.uDirection.CurrentDirection);
        uMain.transform.position = new Vector2(uMain.transform.position.x + climbOffset.x * directionSign, uMain.transform.position.y + climbOffset.y);

        var stateManager = uMain.uState;

        if (stateManager.IsCrouchPerformed && Mathf.Abs(stateManager.MoveInput.x) > 0.1f)
        {
            stateManager.SwitchState(UNITSTATE.CROUCH);
        }
        else if (stateManager.IsRunPerformed && Mathf.Abs(stateManager.MoveInput.x) > 0.1f)
        {
            stateManager.SwitchState(UNITSTATE.RUN);
        }
        else if (Mathf.Abs(stateManager.MoveInput.x) > 0.1f)
        {
            stateManager.SwitchState(UNITSTATE.WALK);
        }
        else if (stateManager.IsCrouchPerformed)
        {
            stateManager.SwitchState(UNITSTATE.IDLECROUCH);
        }
        else
        {
            stateManager.SwitchState(UNITSTATE.IDLE);
        }
    }
}

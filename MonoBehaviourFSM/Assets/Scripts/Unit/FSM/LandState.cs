using UnityEngine;

public class LandState : UnitStateBase
{
    public override UNITSTATE StateType => UNITSTATE.LAND;

    protected override IMovementStrategy MovementStrategy { get; } = new DefaultMovementStrategy();
    
    private EndAnimationBehaviour endAnimBehaviour;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        
        // Find the EndAnimationBehaviour attached to the relevant state
        var animator = uMain.uAnimator.Animator; // Adjust as needed for your setup
        foreach (var behaviour in animator.GetBehaviours<EndAnimationBehaviour>())
        {
            if (behaviour.stateName == "Land")
            {
                endAnimBehaviour = behaviour;
                break;
            }
        }

        if (endAnimBehaviour != null)
        {
            endAnimBehaviour.OnEndAnimation = (anim, stateInfo, layerIndex) => FinishLanding();
        }

        uMain.uAnimator.SetAnimatorTrigger("Land");
        uMain.uState.SetActualDirection();
    }

    public override void Exit()
    {
        if (endAnimBehaviour != null)
        {
            endAnimBehaviour.OnEndAnimation = null;
        }
        base.Exit();
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

    private void FinishLanding()
    {
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

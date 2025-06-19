using UnityEngine;

public class GrabPlatformState : UnitStateBase
{
    [SerializeField] private float grabOffsetY;
    [SerializeField] private float jumpOffsetY;

    public override UNITSTATE StateType => UNITSTATE.GRABPLATFORM;
    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();

    private StartAnimationBehaviour startAnimBehaviour;
    private EndAnimationBehaviour endAnimBehaviour;

    private bool grabFinished = false;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);

        // Find the EndAnimationBehaviour attached to the relevant state
        var animator = uMain.uAnimator.Animator; // Adjust as needed for your setup
        foreach (var behaviour in animator.GetBehaviours<StartAnimationBehaviour>())
        {
            if (behaviour.stateName == "GrabLedge")
            {
                startAnimBehaviour = behaviour;
                break;
            }
        }
        if (startAnimBehaviour != null)
        {
            startAnimBehaviour.OnStartAnimation = (anim, stateInfo, layerIndex) => SnapToPlatform();
        }

        foreach (var behaviour in animator.GetBehaviours<EndAnimationBehaviour>())
        {
            if (behaviour.stateName == "GrabLedge")
            {
                endAnimBehaviour = behaviour;
                break;
            }
        }
        if (endAnimBehaviour != null)
        {
            endAnimBehaviour.OnEndAnimation = (anim, stateInfo, layerIndex) => FinishGrabbing();
        }

        uMain.uAnimator.SetAnimatorTrigger("GrabLedge");

        // Stop all movement
        uMain.rb.linearVelocity = Vector2.zero;
    }

    public override void OnMove(Vector2 direction)
    {
        if (!grabFinished) return;
        if (direction.y < 0)
        {
            HandleDrop();
            return;
        }

        if (direction.y > 0 || direction.x != 0)
        {
            HandleClimb();
            return;
        }
    }

    private void HandleClimb()
    {
        uMain.uState.SwitchState(UNITSTATE.CLIMBLEDGE);
    }

    private void HandleDrop()
    {
        uMain.uCollisions.IgnoreLedge();
        uMain.uState.SwitchState(UNITSTATE.FALL);
    }

    private void SnapToPlatform()
    {
        // Snap to ledge position
        var ledge = uMain.uCollisions.Ledge;
        if (ledge != null)
        {
            var platformCollider = ledge.GetComponent<BoxCollider2D>();
            if (platformCollider != null)
            {
                // Get platform world bounds
                Bounds platformBounds = platformCollider.bounds;

                float xPos = Mathf.Clamp(uMain.transform.position.x, platformBounds.min.x, platformBounds.max.x);
                float yPos = platformBounds.max.y + grabOffsetY;

                uMain.transform.position = new Vector2(xPos, yPos);
            }
        }
    }

    public override void OnJump()
    {
        var ledge = uMain.uCollisions.Ledge;
        if (ledge != null)
        {
            var platformCollider = ledge.GetComponent<BoxCollider2D>();
            if (platformCollider != null)
            {
                // Get platform world bounds
                Bounds platformBounds = platformCollider.bounds;

                uMain.transform.position = new Vector2(uMain.transform.position.x, platformBounds.max.y + jumpOffsetY);
            }
        }

        uMain.uState.SwitchState(UNITSTATE.JUMP);
    }

    private void FinishGrabbing()
    {
        grabFinished = true;
        if (uMain.uState.MoveInput.y > 0 || uMain.uState.MoveInput.x != 0)
        {
            HandleClimb();
            return;
        }
    }
}

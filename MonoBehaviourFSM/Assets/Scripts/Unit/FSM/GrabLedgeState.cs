using UnityEngine;

public class GrabLedgeState : UnitStateBase
{
    [SerializeField] private Vector2 grabOffset;
    [SerializeField] private float skipLedgeOffsetX = 0.5f; // Offset to adjust the position when grabbing the wall

    public override UNITSTATE StateType => UNITSTATE.GRABLEDGE;
    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();
   
    private StartAnimationBehaviour startAnimBehaviour;
    private EndAnimationBehaviour endAnimBehaviour;

    private bool grabFinished = false;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);
        grabFinished = false;
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
            startAnimBehaviour.OnStartAnimation = (anim, stateInfo, layerIndex) => SnapToLedge();
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

        bool isClimbInput = direction.y > 0 ||
            (!Mathf.Approximately(direction.x, 0) && Mathf.Sign(direction.x) == Mathf.Sign((float)uMain.uDirection.CurrentDirection));
        
        if (isClimbInput)
        {
            HandleClimb();
            return;
        }
       
        
        bool isDropInput = direction.y < 0 ||
            (!Mathf.Approximately(direction.x, 0) && Mathf.Sign(direction.x) != Mathf.Sign((float)uMain.uDirection.CurrentDirection));

        if (isDropInput)
        {
            HandleDrop();
            return;
        }
    }

    private void HandleClimb()
    {
        uMain.uState.SwitchState(UNITSTATE.CLIMBLEDGE);
    }

    private void HandleDrop()
    {
        var ledge = uMain.uCollisions.Ledge;
        if (ledge != null)
        {
            var platformCollider = ledge.GetComponent<BoxCollider2D>();
            if (platformCollider != null)
            {
                Bounds platformBounds = platformCollider.bounds;

                int dir = (int)uMain.uDirection.CurrentDirection;
                float xPos = dir > 0
                    ? platformBounds.min.x - skipLedgeOffsetX
                    : platformBounds.max.x + skipLedgeOffsetX;

                uMain.transform.position = new Vector2(xPos, uMain.transform.position.y);
            }
        }

        uMain.uDirection.ReverseDirection();
        uMain.uCollisions.IgnoreLedge();
        uMain.uState.SwitchState(UNITSTATE.FALL);
    }

    private void SnapToLedge()
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

                // Determine which side to snap to (left or right edge of platform)
                int dir = (int)uMain.uDirection.CurrentDirection;
                float xPos = dir > 0
                    ? platformBounds.min.x - grabOffset.x // right edge
                    : platformBounds.max.x + grabOffset.x; // left edge

                // Y: align hands with top of platform
                float yPos = platformBounds.max.y + grabOffset.y;

                uMain.transform.position = new Vector2(xPos, yPos);
            }
        }
    }

    public override void OnJump()
    {
        // Jump away from the ledge
        uMain.uState.SwitchState(UNITSTATE.JUMPWALL);
    }
    
    private void FinishGrabbing()
    {
        grabFinished = true;
        bool isClimbInput = uMain.uState.MoveInput.y > 0 ||
            (uMain.uState.MoveInput.x != 0 && Mathf.Sign(uMain.uState.MoveInput.x) == Mathf.Sign((float)uMain.uDirection.CurrentDirection));

        if (isClimbInput)
        {
            HandleClimb();
            return;
        }
    }
}
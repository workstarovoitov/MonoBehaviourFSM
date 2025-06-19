using UnityEngine;

public class GrabWallState : UnitStateBase
{
    //[SerializeField] private float xOffset = 0.5f; // Offset to adjust the position when grabbing the wall

    public override UNITSTATE StateType => UNITSTATE.GRABWALL;
    protected override IMovementStrategy MovementStrategy { get; } = new ConstantSpeedMovementStrategy();

    private StartAnimationBehaviour startAnimBehaviour;

    public override void Enter(UnitMain unitMain)
    {
        base.Enter(unitMain);

        // Find the EndAnimationBehaviour attached to the relevant state
        var animator = uMain.uAnimator.Animator; // Adjust as needed for your setup

        foreach (var behaviour in animator.GetBehaviours<StartAnimationBehaviour>())
        {
            if (behaviour.stateName == "GrabWall")
            {
                startAnimBehaviour = behaviour;
                break;
            }
        }
        if (startAnimBehaviour != null)
        {
            startAnimBehaviour.OnStartAnimation = (anim, stateInfo, layerIndex) => SnapToWall();
        }


        uMain.uAnimator.SetAnimatorTrigger("GrabWall");
        movementContext.MaxSpeed = new Vector2(0, -movementSettings.MaxSpeed.y);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (TryLand()) return;
        if (TryFall()) return;
    }

    private bool TryLand()
    {
        if (uMain.uState.IsGrounded)
        {
            uMain.uState.SwitchState(UNITSTATE.LAND);
            return true;
        }
        return false;
    }

    private bool TryFall()
    {
        if (!uMain.uCollisions.WallForGrabInFrontX())
        {
            uMain.uState.SwitchState(UNITSTATE.FALL);
            return true;
        }
        return false;
    }

    public override void OnMove(Vector2 direction)
    {
        if (direction.y != 0)
        {
            uMain.uState.SwitchState(UNITSTATE.CLIMBWALL);
        }

        if (direction.x != 0 && Mathf.Sign(direction.x) != Mathf.Sign((float)uMain.uDirection.CurrentDirection))
        {
            uMain.uDirection.ReverseDirection();
            uMain.uState.SwitchState(UNITSTATE.FALL);
        }
    }

    public override void OnJump()
    {
        uMain.uState.SwitchState(UNITSTATE.JUMPWALL);
    }

    private void SnapToWall()
    {
        var wallObj = uMain.uCollisions.Wall;
        if (wallObj != null)
        {
            var wallCollider = wallObj.GetComponent<Collider2D>();
            var unitCollider = uMain.bc;
            if (wallCollider != null && unitCollider != null)
            {
                Bounds wallBounds = wallCollider.bounds;
                Bounds unitBounds = unitCollider.bounds;
                int dir = (int)uMain.uDirection.CurrentDirection;

                // Snap the unit so its edge is xOffset away from the wall's edge
                float unitHalfWidth = unitBounds.size.x / 2f + unitCollider.edgeRadius;
                float newX = dir > 0
                    ? wallBounds.min.x - unitHalfWidth  // Facing right, snap to left edge of wall
                    : wallBounds.max.x + unitHalfWidth; // Facing left, snap to right edge of wall

                uMain.transform.position = new Vector2(newX, uMain.transform.position.y);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION
{
    RIGHT = 1,
    LEFT = -1
}

public class UnitDirectionManager : MonoBehaviour
{
    [SerializeField]
    private DIRECTION currentDirection = DIRECTION.RIGHT;
    public DIRECTION CurrentDirection => currentDirection;

    [SerializeField]
    private List<UNITSTATE> statesReadyToRotate = new List<UNITSTATE>
    {
        UNITSTATE.IDLE,
        UNITSTATE.WALK,
        UNITSTATE.JUMP,
        UNITSTATE.JUMPSECOND,
        UNITSTATE.FALL,
        UNITSTATE.ATTACK,
        UNITSTATE.HIDE,
    };

    private UnitMain uMain;

    public void Initialize(UnitMain unitMain)
    {
        if (unitMain == null)
        {
            Debug.LogError("UnitMain reference is null in UnitDirectionManager.");
            enabled = false;
            return;
        }

        uMain = unitMain;
        enabled = true;
        RotateCharacterObject();
    }

    /// <summary>
    /// Sets current direction and runs revert animation if needed.
    /// </summary>
    public void UpdateDirection()
    {
        if (uMain.uState.MoveInput.x == 0)
            return;

        if (statesReadyToRotate == null ||
            statesReadyToRotate.Count == 0 ||
            statesReadyToRotate.Contains(uMain.uState.CurrentState))
        {
            currentDirection = uMain.uState.MoveInput.x < 0 ? DIRECTION.LEFT : DIRECTION.RIGHT;
            RotateCharacterObject();
        }
    }

    public void UpdateDirectionForced()
    {
        currentDirection = uMain.uState.MoveInput.x < 0 ? DIRECTION.LEFT : DIRECTION.RIGHT;
        RotateCharacterObject();
    }

    public void SetDirection(DIRECTION direction)
    {
        if (direction == currentDirection)
            return;

        currentDirection = direction;
        RotateCharacterObject();
    }

    public void ReverseDirection()
    {
        currentDirection = (DIRECTION)((int)currentDirection * -1);
        RotateCharacterObject();
    }

    /// <summary>
    /// Flips character 180 degrees to current direction.
    /// </summary>
    public void RotateCharacterObject()
    {
        float flip = currentDirection == DIRECTION.RIGHT ? 0f : 180f;
        transform.rotation = Quaternion.Euler(Vector3.up * flip);
    }
}

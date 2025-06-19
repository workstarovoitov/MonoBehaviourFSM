using UnityEngine;

public interface IMovementStrategy
{
    void ApplyMovement(UnitMain uMain, MovementContext context);
}

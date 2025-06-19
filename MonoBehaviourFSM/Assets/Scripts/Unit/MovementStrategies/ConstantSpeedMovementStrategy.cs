using UnityEngine;

/// <summary>
/// Movement strategy that sets the unit's velocity to a constant value, ignoring input and acceleration.
/// </summary>
public class ConstantSpeedMovementStrategy : IMovementStrategy
{
    /// <summary>
    /// Applies a constant velocity to the unit, using the context's CustomVelocity if provided.
    /// </summary>
    /// <param name="uMain">The main unit object.</param>
    /// <param name="context">Movement context containing the custom velocity.</param>
    public void ApplyMovement(UnitMain uMain, MovementContext context)
    {
        if (uMain == null || context == null)
            return;

        Vector2 velocity = context.MaxSpeed;
        uMain.rb.linearVelocity = velocity;
    }
}

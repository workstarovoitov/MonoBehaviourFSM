using UnityEngine;

/// <summary>
/// Handles jump movement logic for a unit, applying horizontal and vertical velocity based on input and context.
/// </summary>
public class JumpMovementStrategy : IMovementStrategy
{
    /// <summary>
    /// Applies jump movement to the unit, considering input, speed settings, and gravity.
    /// </summary>
    /// <param name="uMain">The main unit object.</param>
    /// <param name="context">Movement context containing speed settings and optional overrides.</param>
    public void ApplyMovement(UnitMain uMain, MovementContext context)
    {
        if (uMain == null || context == null)
            return;

        float moveInputX = uMain.uState.MoveInput.x;
        bool isMoveInputActive = !Mathf.Approximately(moveInputX, 0f);

        float directionSign = Mathf.Sign((float)uMain.uDirection.CurrentDirection);
        float maxSpeed = context.MaxSpeed.x;
        float acceleration = isMoveInputActive ? context.AccelerationX : context.DeccelerationX;

        float velocityX = uMain.rb.linearVelocity.x;

        // Horizontal movement logic
        if (isMoveInputActive && (Mathf.Abs(velocityX) < maxSpeed || directionSign * velocityX < 0))
        {
            velocityX += directionSign * acceleration;
            velocityX = Mathf.Clamp(velocityX, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocityX > 0)
                velocityX = Mathf.Max(0, velocityX - acceleration);
            else if (velocityX < 0)
                velocityX = Mathf.Min(0, velocityX + acceleration);
        }

        uMain.rb.linearVelocity = new Vector2(velocityX, context.MaxSpeed.y);
    }
}

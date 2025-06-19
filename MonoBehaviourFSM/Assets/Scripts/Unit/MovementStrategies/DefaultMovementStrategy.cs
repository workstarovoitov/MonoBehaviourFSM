using UnityEngine;

/// <summary>
/// Default movement strategy for ground movement, applying acceleration, deceleration, and clamping speed.
/// </summary>
public class DefaultMovementStrategy : IMovementStrategy
{
    /// <summary>
    /// Applies default ground movement logic to the unit.
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

        float gravityScale = uMain.rb.linearVelocity.y < 0 ? uMain.GravityScaleFalling : uMain.GravityScale;
        float velocityY = uMain.rb.linearVelocity.y + uMain.GlobalGravity * gravityScale;
        velocityY = Mathf.Clamp(velocityY, -context.MaxSpeed.y, context.MaxSpeed.y);

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

        uMain.rb.linearVelocity = new Vector2(velocityX, velocityY);
    }
}

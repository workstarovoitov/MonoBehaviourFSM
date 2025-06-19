using UnityEngine;

[System.Serializable]
public class MovementContext
{
    public Vector2 MaxSpeed;

    public float AccelerationX;
    public float DeccelerationX;

    public MovementContext(MovementContext movementContext)
    {
        MaxSpeed = movementContext.MaxSpeed;
        AccelerationX = movementContext.AccelerationX;
        DeccelerationX = movementContext.DeccelerationX;
    }
}

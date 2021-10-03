using Godot;
using System;

public class TempEnemy : Sprite
{
    public Vector2 Velocity => Position.DirectionTo(TargetLocation) * Speed;

    [Export]
    public Single Speed { get; private set; } = 200f;

    private Vector2 TargetLocation = Vector2.Zero;

    public override void _Ready()
    {
        AddToGroup("Targetable");

        Position = NewTarget();
        TargetLocation = NewTarget();
    }

    public override void _Process(Single delta)
    {
        LookAt(TargetLocation);
        RotationDegrees -= 90f;
        Position += Position.DirectionTo(TargetLocation) * Speed * delta;
        if (Position.DistanceTo(TargetLocation) < Speed * 2f)
        {
            TargetLocation = NewTarget();
        }
    }

    /// <summary>
    /// Estimated future position
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns>Position</returns>
    public Vector2 Forecast(Single time)
    {
        return GlobalPosition + Velocity * time;
    }

    private Vector2 NewTarget() => new Vector2((Single)GD.RandRange(-1920, 1920), (Single)GD.RandRange(-1080, 1080));
}

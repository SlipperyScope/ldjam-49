using Godot;
using System;

public class TempEnemy : Sprite
{
    [Export]
    public Vector2 Velocity { get; private set; }

    public override void _Ready()
    {
        LookAt(Position + Velocity);
        RotationDegrees += 90f;
        AddToGroup("Targetable");
    }

    public override void _Process(Single delta)
    {
        Position += Velocity * delta;
    }

    /// <summary>
    /// Estimated future position
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns>Position</returns>
    public Vector2 Forecast(Single time)
    {
        return Position + Velocity * time;
    }
}

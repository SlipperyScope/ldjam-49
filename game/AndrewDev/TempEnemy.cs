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
        AddToGroup("Enemies");
    }

    public override void _Process(Single delta)
    {
        Position += Velocity * delta;
    }
}

using Godot;
using System;

public class Turret : Node2D
{
    public Node2D Base { get; private set; }
    public Node2D Gun { get; private set; }

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        Base = GetNode<Node2D>("Base");
        Gun = GetNode<Node2D>("Gun");
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        Aim();
        Fire();
    }

    private void Aim()
    {
        var target = GetTarget();

        Gun.LookAt(target);
    }

    private Vector2 GetTarget()
    {
        return GetGlobalMousePosition();
    }

    private void Fire()
    {

    }
}

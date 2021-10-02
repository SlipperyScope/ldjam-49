using Godot;
using System;

public class Bullet : Node2D
{
    const String SpritePath = "Sprite";

    public Vector2 Velocity { get; set; }

    private Sprite Sprite;

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        Sprite = GetNode<Sprite>(SpritePath);
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        Position += Velocity * delta;
    }
}

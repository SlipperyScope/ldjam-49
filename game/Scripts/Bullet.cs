using Godot;
using System;

public class Bullet : Node2D
{
    const String SpritePath = "Sprite";

    public Vector2 Velocity { get; set; }

    public Node2D Target { get; set; }
    
    private Sprite Sprite;

    public Single Damage;
    public Boolean Penetrates;
    public Single SpriteRotation
    {
        get => Sprite.Rotation;
        set => Sprite.Rotation = value;
    }

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

    /// <summary>
    /// Estimated future position
    /// </summary>
    /// <param name="time">Time in seconds</param>
    /// <returns>Position</returns>
    public Vector2 Forecast(Single time)
    {
        return ToGlobal(Position + Velocity * time);
    }
}

using Godot;
using System;

public class Bullet : Area2D
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

    public override void _EnterTree()
    {
        Connect("area_entered", this, nameof(OnAreaEntered));

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
        if (Position.x > 1920f || Position.x < -1920f || Position.y > 1080f || Position.y < -1080f)
        {
            QueueFree();
        }
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is SeaAnemone && Penetrates is false)
        {
            QueueFree();
        }
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

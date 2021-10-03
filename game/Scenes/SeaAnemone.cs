using Godot;
using System;

public class SeaAnemone : Area2D
{
    const String ExplodePlayerPath = "ExplodeSound";

    public AudioStreamPlayer ExplodePlayer;

    public Vector2 Velocity { get; private set; }

    public Single Speed = 400f;

    public Vector2 TargetLocation
    {
        get => _TargetLocation;
        set
        {
            _TargetLocation = value;
            OnTargetChanged();
        }
    }
    private Vector2 _TargetLocation;
    public override void _EnterTree()
    {
        Connect("area_entered", this, nameof(OnAreaEntered));
        AddToGroup("Targetable");
    }

    public override void _Ready()
    {
        ExplodePlayer = GetNode<AudioStreamPlayer>(ExplodePlayerPath);
        ExplodePlayer.Connect("finished", this, nameof(Sewercide));
        Position = NewTarget();
        TargetLocation = NewTarget();
    }

    public override void _Process(Single delta)
    {
        Position += Velocity * delta;
        if (Position.DistanceTo(TargetLocation) < Speed * 2f)
        {
           TargetLocation = NewTarget();
        }
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is TurretController)
        {
            //Do damage to turret
        }
        GD.Print($"{this}={Name} collided with {other}");
        ExplodePlayer.Play();
        Visible = false;
        var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        collisionShape.CallDeferred("set", "disabled", true);
    }

    private void OnTargetChanged()
    {
        LookAt(TargetLocation);
        Velocity = Position.DirectionTo(TargetLocation) * Speed;
    }

    public Vector2 Forecast(Single time)
    {
        return GlobalPosition + Velocity * time;
    }

    private void Sewercide()
    {
        //QueueFree();
        Position = NewTarget();
        TargetLocation = NewTarget();
        Visible = true;
        var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        collisionShape.CallDeferred("set", "disabled", false);
    }

    // temp target aquisition
    private Vector2 NewTarget() => new Vector2((Single)GD.RandRange(-1920, 1920), (Single)GD.RandRange(-1080, 1080));
}

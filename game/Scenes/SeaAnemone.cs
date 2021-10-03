using Godot;
using System;

public class SeaAnemone : Area2D
{
    const String ExplodePlayerPath = "ExplodeSound";
    const String HitPlayerPath = "HitSound";
    const String ShapePath = "CollisionShape2D";

    public AudioStreamPlayer2D ExplodePlayer { get; private set; }
    public AudioStreamPlayer2D HitPlayer { get; private set; }
    public CollisionShape2D Collision { get; private set; } 

    public Vector2 Velocity { get; private set; }

    public Single Speed = 400f;

    public Int32 HP = 4;

    public Single StabilityCost = 10f;

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
        ExplodePlayer = GetNode<AudioStreamPlayer2D>(ExplodePlayerPath);
        HitPlayer = GetNode<AudioStreamPlayer2D>(HitPlayerPath);
        Collision = GetNode<CollisionShape2D>(ShapePath);

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
        switch (other)
        {
            case TurretController turret:
                turret.AddStability(-StabilityCost);
                Damage(HP);
                break;
            case SeaAnemone enemy:
                Damage(HP);
                break;
            case Bullet bullet:
                Damage(1);
                HitPlayer.Play(0.1f);
                break;
            default:
                break;
        }
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

    public void Damage(Int32 amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Visible = false;
        Collision.CallDeferred("set", "disabled", true);
        ExplodePlayer.Play(0.1f);
    }

    private void Sewercide()
    {
        //QueueFree();
        Position = NewTarget();
        TargetLocation = NewTarget();
        Visible = true;
        Collision.CallDeferred("set", "disabled", false);
        HP = 4;
    }

    // temp target aquisition
    private Vector2 NewTarget() => new Vector2((Single)GD.RandRange(-1920, 1920), (Single)GD.RandRange(-1080, 1080));
}

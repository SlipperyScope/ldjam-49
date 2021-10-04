using Godot;
using System;

public class SeaAnemone : Area2D
{
    const String ExplodePlayerPath = "ExplodeSound";
    const String HitPlayerPath = "HitSound";
    const String ShapePath = "CollisionShape2D";
    const String GlobalDataPath = "/root/GlobalData";

    private Boolean ExplosionSoundFinished = false;
    private Boolean ExplosionAnimationFinished = false;
    private Boolean DED = false;

    public AudioStreamPlayer2D ExplodePlayer { get; private set; }
    public AudioStreamPlayer2D HitPlayer { get; private set; }
    public CollisionShape2D Collision { get; private set; }
    public GlobalData Global { get; private set; } 

    public Vector2 Velocity { get; private set; }

    /// <summary>
    /// Enemy move speed in px/s
    /// </summary>
    [Export]
    public Single Speed = 400f;

    /// <summary>
    /// Bullet hits before death
    [Export]
    /// </summary>
    public Int32 HP = 4;

    /// <summary>
    /// Damage to turret on hit
    [Export]
    /// </summary>
    public Single StabilityCost = 5f;

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

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        ExplodePlayer = GetNode<AudioStreamPlayer2D>(ExplodePlayerPath);
        HitPlayer = GetNode<AudioStreamPlayer2D>(HitPlayerPath);
        Collision = GetNode<CollisionShape2D>(ShapePath);
        Global = GetNode<GlobalData>(GlobalDataPath);

        ExplodePlayer.Connect("finished", this, nameof(OnExplosionSoundFinished));

        //Position = NewTarget();
        //TargetLocation = NewTarget();
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        Position += Velocity * delta;
        //if (Position.DistanceTo(TargetLocation) < Speed * 2f)
        //{
        //   TargetLocation = NewTarget();
        //}
    }

    /// <summary>
    /// On area entered
    /// </summary>
    /// <param name="other">Shape that entered</param>
    private void OnAreaEntered(Area2D other)
    {
        switch (other)
        {
            case TurretController turret:
                turret.AddStability(-StabilityCost);
                Explode();
                Global.EnemyHits++;
                break;
            case SeaAnemone enemy:
                //Damage(HP);
                break;
            case Bullet bullet:
                CallDeferred(nameof(Damage), 1);
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
        var projection = GlobalPosition + Velocity * time;
        var distanceToTarget = GlobalPosition.DistanceSquaredTo(TargetLocation);
        if (projection.LengthSquared() > distanceToTarget)
        {
            projection = GlobalPosition;
        }
        return projection;
    }

    /// <summary>
    /// Do damage to enemy HP
    /// </summary>
    /// <param name="amount"></param>
    public void Damage(Int32 amount)
    {
        HP -= amount;
        Global.Hits++;
        if (HP <= 0)
        {
            Explode();
            Global.Kills++;
        }
    }

    private void Explode()
    {
        if (DED is true) return;
        DED = true;
        Velocity = Vector2.Zero;
        var children = GetChildren();
        foreach (Node2D child in children)
        {
            if (child.Name != "Explosion")
            {
                child.Visible = false;
            }
            if (child.Name == "Explosion" && child is AnimatedSprite explosion)
            {
                explosion.Visible = true;
                explosion.Connect("animation_finished", this, nameof(OnExplosionAnimationFinished));
                explosion.Frame = 0;
                explosion.Play();
            }
        }
        Collision.CallDeferred("set", "disabled", true);
        ExplodePlayer.Play(0.1f);
    }

    private void OnExplosionAnimationFinished()
    {
        if (ExplosionSoundFinished is true)
        {
            Sewercide();
        }
        else
        {
            ExplosionAnimationFinished = true;
        }
        foreach (Node2D child in GetChildren())
        {
            if (child.Name == "Explosion" && child is AnimatedSprite explosion)
            {
                explosion.Visible = false;
            }
        }
    }

    private void OnExplosionSoundFinished()
    {
        if (ExplosionAnimationFinished is true)
        {
            Sewercide();
        }
        else
        {
            ExplosionSoundFinished = true;
        }
    }

    private void Sewercide()
    {
        QueueFree();
        //Position = NewTarget();
        //TargetLocation = NewTarget();
        //Visible = true;
        //Collision.CallDeferred("set", "disabled", false);
        //HP = 4;
    }

    // temp target aquisition
    private Vector2 NewTarget() => new Vector2((Single)GD.RandRange(-1920, 1920), (Single)GD.RandRange(-1080, 1080));
}

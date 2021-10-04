using Godot;
using System;

public class SeaAnemone : Area2D
{
    const String ExplodePlayerPath = "ExplodeSound";
    const String HitPlayerPath = "HitSound";
    const String ShapePath = "CollisionShape2D";
    const String GlobalDataPath = "/root/GlobalData";

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

        ExplodePlayer.Connect("finished", this, nameof(Sewercide));
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
        Visible = false;
        Collision.CallDeferred("set", "disabled", true);
        ExplodePlayer.Play(0.1f);
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

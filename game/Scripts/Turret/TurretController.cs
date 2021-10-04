using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TurretController : Area2D
{
    const String BulletScenePath = "res://Scenes/Bullet.tscn";

    const String BasePath = "Base";
    const String GunPath = "Gun";
    const String OuchPlayerPath = "Ouch";
    const String DeadPlayerPath = "Dead";
    public AudioStreamPlayer OuchPlayer { get; private set; }
    public AudioStreamPlayer DeadPlayer { get; private set; }

    [Export]
    private NodePath TargetAreaPath = "TargetArea";
    public Area2D TargetArea { get; private set; }
    public List<SeaAnemone> TargetableEnemies { get; private set; } = new List<SeaAnemone>();

    /// <summary>
    /// Path to attackr educer
    /// </summary>
    [Export]
    public NodePath AttackReducerPath { get; private set; }

    /// <summary>
    /// Turret base
    /// </summary>
    public Node2D Base { get; private set; }

    /// <summary>
    /// Turret gun
    /// </summary>
    public FireController Gun { get; private set; }

    /// <summary>
    /// Turret target mode
    /// </summary>
    public enum TargetMode
    {
        Nearest,
        Strongest,
        Deadest
    }

    /// <summary>
    /// Max rotation speed of turret
    /// </summary>
    public Single RotationSpeed { get; private set; } = 720f;

    /// <summary>
    /// Current Target mode
    /// </summary>
    public TargetMode Mode = TargetMode.Nearest;

    /// <summary>
    /// Current Target
    /// </summary>
    private SeaAnemone Target;

    public Single MaxStability = 100f;
    public Single CurrentStability = 50f;
    public Single RecoveryRate = 2f;
    public bool Alive = true;

    public delegate void DedHandler(object sender);
    public event DedHandler DidDed;

    public override void _EnterTree()
    {
        Connect("area_entered", this, nameof(OnAreaEntered));
    }

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        RotationSpeed = (Single)(RotationSpeed * Math.PI / 180f);

        OuchPlayer = GetNode<AudioStreamPlayer>(OuchPlayerPath);
        DeadPlayer = GetNode<AudioStreamPlayer>(DeadPlayerPath);
        Base = GetNode<Node2D>(BasePath);
        Gun = GetNode<FireController>(GunPath);
        TargetArea = GetNode<Area2D>(TargetAreaPath);
        TargetArea.Connect("area_entered", this, nameof(OnTargetAreaEntered));
        TargetArea.Connect("area_exited", this, nameof(OnTargetAreaExited));

        var reducer = GetNode<AttackReducer>(AttackReducerPath);
        reducer.AttackDefinitionUpdated += Reducer_AttackDefinitionUpdated;
    }

    private void Reducer_AttackDefinitionUpdated(System.Object sender, AttackDefinitionUpdatedArgs e)
    {
        var definition = e.Definition;
        Gun.BurstSize = definition.burstCount;
        Gun.BurstInterval = definition.burstDelay;
        Gun.BurstCooldown = definition.shotDelay;
        Gun.DoHorn = definition.canHasHorn;
        Gun.Projectiles = definition.projectiles;
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        if (Alive) {
            Aim(delta);
            Recover(delta);
        }
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is SeaAnemone)
        {
            OuchPlayer.Play(0.1f);
        }
    }

    /// <summary>
    /// On target area entered
    /// </summary>
    /// <param name="other">Body entering target area</param>
    private void OnTargetAreaEntered(Area2D other)
    {
        if (other is SeaAnemone enemy)
        {
            TargetableEnemies.Add(enemy);
            if (TargetableEnemies.Count == 1 && Alive)
            {
                Gun.DoFire = true;
            }
        }
    }

    /// <summary>
    /// On target area exited
    /// </summary>
    /// <param name="other">body exiting target area</param>
    private void OnTargetAreaExited(Area2D other)
    {
        if (other is SeaAnemone enemy)
        {
            TargetableEnemies.Remove(enemy);
            if (TargetableEnemies.Count == 0)
            {
                Gun.DoFire = false;
            }
        }
    }

    /// <summary>
    /// Aims the turret
    /// </summary>
    /// <param name="delta">Time delta</param>
    /// <param name="LimitRotationSpeed">Limit speed turret can rotate</param>
    private void Aim(Single delta, Boolean LimitRotationSpeed = true)
    {
        Target = Mode switch
        {
            TargetMode.Nearest => NearestTarget(),
            TargetMode.Strongest => throw new NotImplementedException(),
            TargetMode.Deadest => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

        // TODO: Make this for not temp enemies
        if (Target is null) return;

        var forecastPosition = Target.GlobalPosition;//(Target as TempEnemy)?.Forecast(Gun.Tip.GlobalPosition.DistanceTo(Target.GlobalPosition) / (Gun.BulletSpeed * 0.8f)) ?? (Target as SeaAnemone)?.Forecast(Gun.Tip.GlobalPosition.DistanceTo(Target.GlobalPosition) / (Gun.BulletSpeed * 0.8f)) ?? Target.Position;

        var angle = Gun.GetAngleTo(forecastPosition);
        var frameRotation = RotationSpeed * delta;

        if (LimitRotationSpeed is true && Math.Abs(angle) > frameRotation)
        {
            Gun.Rotation += angle < 0f ? -frameRotation : frameRotation;
        }
        else
        {
            Gun.LookAt(forecastPosition);
        }
    }

    /// <summary>
    /// Get Targets
    /// </summary>
    /// <returns>List of targets</returns>
    private List<Node2D> GetTargets()
    {
        var targets = GetTree().GetNodesInGroup("Targetable");
        List<Node2D> targetsList = new List<Node2D>();

        foreach (Node2D target in targets)
        {
            if (target != null)
            {
                targetsList.Add(target);
            }
        }

        return targetsList;
    }

    public void AddStability(Single value)
    {
        CurrentStability += value;
        if (CurrentStability <= 0 && Visible is true)
        {
            GD.Print($"omglookimdead");
            GetNode<CollisionShape2D>("CollisionShape2D").CallDeferred("set", "disabled", true);
            DeadPlayer.Play(0.1f);
            Visible = false;
            this.Alive = false;
            this.Gun.DoFire = false;
            this.DidDed?.Invoke(this);
        }
        else if (CurrentStability > MaxStability)
        {
            CurrentStability = MaxStability;
        }
        //GD.Print($"hit");
    }
    /// <summary>
    /// Nearest target
    /// </summary>
    /// <returns>Target nearest to the turret</returns>
    private SeaAnemone NearestTarget() =>
        TargetableEnemies.OrderBy(e => GlobalPosition.DistanceSquaredTo(e.GlobalPosition)).FirstOrDefault();
        //GetTargets().FindAll(target => target.Visible is true).OrderBy(target => GlobalPosition.DistanceSquaredTo(target.GlobalPosition)).FirstOrDefault();

    private void Recover(Single delta) {
        if (this.CurrentStability < this.MaxStability) {
            this.CurrentStability += Math.Min(this.RecoveryRate * delta, this.MaxStability - this.CurrentStability);
        }
    }
}

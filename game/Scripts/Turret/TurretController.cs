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
    public enum TargetMode    {        Nearest,        Strongest,        Deadest    }

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
    private Node2D Target;

    public Single MaxStability = 100f;
    public Single CurrentStability = 40f;
    public Single RecoveryRate = 0.5f;

    public override void _EnterTree()
    {
        Connect("area_entered", this, nameof(OnAreaEntered));
    }

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()    {        RotationSpeed = (Single)(RotationSpeed * Math.PI / 180f);
        OuchPlayer = GetNode<AudioStreamPlayer>(OuchPlayerPath);        DeadPlayer = GetNode<AudioStreamPlayer>(DeadPlayerPath);        Base = GetNode<Node2D>(BasePath);        Gun = GetNode<FireController>(GunPath);        var reducer = GetNode<AttackReducer>(AttackReducerPath);        reducer.AttackDefinitionUpdated += Reducer_AttackDefinitionUpdated;    }    private void Reducer_AttackDefinitionUpdated(System.Object sender, AttackDefinitionUpdatedArgs e)    {        var definition = e.Definition;        Gun.BurstSize = definition.burstCount;        Gun.BurstInterval = definition.burstDelay;        Gun.BurstCooldown = definition.shotDelay;        Gun.DoHorn = definition.canHasHorn;        Gun.Projectiles = definition.projectiles;    }    /// <summary>    /// Process    /// </summary>
    public override void _Process(Single delta)
    {
        Aim(delta);
        Recover(delta);
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is SeaAnemone)
        {
            OuchPlayer.Play(0.1f);
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
        if (Target as TempEnemy is null && Target as SeaAnemone is null) return;

        var forecastPosition = (Target as TempEnemy)?.Forecast(Gun.Tip.GlobalPosition.DistanceTo(Target.GlobalPosition) / (Gun.BulletSpeed * 0.8f)) ?? (Target as SeaAnemone)?.Forecast(Gun.Tip.GlobalPosition.DistanceTo(Target.GlobalPosition) / (Gun.BulletSpeed * 0.8f)) ?? Target.Position;

        var angle = Gun.GetAngleTo(forecastPosition);
        var frameRotation = RotationSpeed * delta;

        if (LimitRotationSpeed is true && Math.Abs(angle) > frameRotation)
        {
            Gun.Rotation -= angle < 0f ? frameRotation : -frameRotation;
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

    public void AddStability(Single value)    {        CurrentStability += value;        if (CurrentStability <= 0 && Visible is true)        {            GD.Print($"omglookimdead");            GetNode<CollisionShape2D>("CollisionShape2D").CallDeferred("set", "disabled", true);            DeadPlayer.Play(0.1f);            Visible = false;        }        else if (CurrentStability > MaxStability)        {            CurrentStability = MaxStability;        }        //GD.Print($"hit");    }
    /// <summary>    /// Nearest target    /// </summary>    /// <returns>Target nearest to the turret</returns>
    private Node2D NearestTarget() =>
        GetTargets().FindAll(target => target.Visible is true).OrderBy(target => GlobalPosition.DistanceSquaredTo(target.GlobalPosition)).FirstOrDefault();

    private void Recover(Single delta) {
        if (this.CurrentStability < this.MaxStability) {
            this.CurrentStability += Math.Min(this.RecoveryRate * delta, this.MaxStability - this.CurrentStability);
        }
    }
}

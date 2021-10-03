using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TurretController : Node2D
{
    const String BulletScenePath = "res://Scenes/Bullet.tscn";

    const String BasePath = "Base";
    const String GunPath = "Gun";

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
    public Single RotationSpeed { get; private set; } = 180f;

    /// <summary>
    /// Current Target mode
    /// </summary>
    public TargetMode Mode = TargetMode.Nearest;

    /// <summary>
    /// Current Target
    /// </summary>
    private Node2D Target;

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        RotationSpeed = (Single)(RotationSpeed * Math.PI / 180f);
        Base = GetNode<Node2D>(BasePath);
        Gun = GetNode<FireController>(GunPath);
        var reducer = GetNode<AttackReducer>(AttackReducerPath);
        reducer.AttackDefinitionUpdated += Reducer_AttackDefinitionUpdated;
    }

    private void Reducer_AttackDefinitionUpdated(System.Object sender, AttackDefinitionUpdatedArgs e)
    {
        var definition = e.Definition;
        Gun.BurstSize = definition.burstCount;
        Gun.BurstInterval = (Single)definition.burstDelay.Yield() / 1000f;
        Gun.BurstCooldown = (Single)definition.shotDelay.Yield() / 1000f;
        Gun.DoHorn = definition.canHasHorn;
       // Gun.Projectiles = definition
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        Aim(delta);
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
        if (Target as TempEnemy is null) return;

        var forecastPosition = (Target as TempEnemy).Forecast(Gun.Tip.GlobalPosition.DistanceTo(Target.GlobalPosition) / (Gun.BulletSpeed * 0.8f));
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

    /// <summary>
    /// Nearest target
    /// </summary>
    /// <returns>Target nearest to the turret</returns>
    private Node2D NearestTarget() =>
        GetTargets().OrderBy(target => GlobalPosition.DistanceSquaredTo(target.GlobalPosition)).FirstOrDefault();

}

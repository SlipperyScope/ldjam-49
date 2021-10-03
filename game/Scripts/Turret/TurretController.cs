using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TurretController : Node2D
{
    const String BulletScenePath = "res://Scenes/Bullet.tscn";

    const String BasePath = "Base";
    const String GunPath = "Gun";

    public Node2D Base { get; private set; }
    public FireController Gun { get; private set; }

    public enum TargetMode
    {
        Nearest,
        Strongest,
        Deadest
    }

    public Single RotationSpeed { get; private set; } = 180f;

    public TargetMode Mode = TargetMode.Nearest;

    private Node2D Target;

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        Base = GetNode<Node2D>(BasePath);
        Gun = GetNode<FireController>(GunPath);
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

        var forecastPosition = (Target as TempEnemy).Forecast(Gun.Tip.GlobalPosition.DistanceTo(Target.GlobalPosition) / Gun.BulletSpeed);
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

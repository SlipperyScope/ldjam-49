using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Turret : Node2D
{
    const Boolean SlowMo = false;
    /// <summary>
    /// Speed of turret rotation in angles per second
    /// </summary>
    [Export]
    public Single RotationSpeed = 180f;

    public Node2D Base { get; private set; }
    public Node2D Gun { get; private set; }
    private Label Output;

    private Int32 debugCount = 0;

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        RotationSpeed = (Single)(RotationSpeed * Math.PI / 180f);
        Base = GetNode<Node2D>("Base");
        Gun = GetNode<Node2D>("Gun");
        Output = GetNode<Label>("/root/Andrew/Label");
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        if (SlowMo && debugCount++ % 30 != 0) return;

        Aim(delta);
        Fire();
    }

    private void Aim(Single delta, Boolean LimitRotationSpeed = true)
    {
        var target = GetTarget();
        if (target is null) return;

        var targetLocation = target.Position;

        if (targetLocation == Position) return;

        if (LimitRotationSpeed is true)
        {
            var angle = Gun.GetAngleTo(targetLocation);
            var frameRotation = RotationSpeed * delta;
            if (Math.Abs(angle) < frameRotation)
            {
                Gun.LookAt(targetLocation);
            }
            else
            {
                Gun.Rotation -= angle < 0f ? frameRotation : -frameRotation;
            }
        }
        else
        {
            Gun.LookAt(targetLocation);
        }
    }

    private Node2D GetTarget()
    {
        var enemies = GetTree().GetNodesInGroup("Enemies");
        List<Node2D> enemyList = new List<Node2D>();
        foreach (var enemy in enemies)
        {
            if (enemy is Node2D)
            {
                enemyList.Add(enemy as Node2D);
            }
        }

        var target = enemyList.OrderBy(enemy => Position.DistanceSquaredTo(enemy.Position)).FirstOrDefault();

        return target;
    }

    private void Fire()
    {

    }
}

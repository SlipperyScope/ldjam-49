using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Turret : Node2D
{
    const String BulletScenePath = "res://Scenes/Bullet.tscn";

    /// <summary>
    /// Speed of turret rotation in angles per second
    /// </summary>
    [Export]
    public Single RotationSpeed = 180f;

    /// <summary>
    /// Time between bursts
    /// </summary>
    [Export]
    public Single FireCooldown = 0.5f;

    /// <summary>
    /// Number of shots per burst
    /// </summary>
    [Export]
    public Int32 BurstSize = 1;

    [Export]
    public Single BurstInterval = 0.05f;

    public Node2D Base { get; private set; }
    public Node2D Gun { get; private set; }
    public Node2D Tip { get; private set; } 

    private Timer ShootTimer;
    private Timer BurstTimer;
    private Boolean ReadyToFire = true;

    private PackedScene BulletScene;

    public override void _EnterTree()
    {
        ShootTimer = new Timer();
        ShootTimer.Connect("timeout", this, nameof(CooldownComplete));
        ShootTimer.WaitTime = FireCooldown;
        AddChild(ShootTimer);

        BulletScene = GD.Load<PackedScene>(BulletScenePath);
    }

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        RotationSpeed = (Single)(RotationSpeed * Math.PI / 180f);
        Base = GetNode<Node2D>("Base");
        Gun = GetNode<Node2D>("Gun");
        Tip = Gun.GetNode<Node2D>("Tip");
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
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
        if (ReadyToFire is false) return;

        var bullet = BulletScene.Instance<Bullet>();
        bullet.Rotation = Gun.GlobalRotation;
        bullet.Velocity = (new Vector2(400f, 0)).Rotated(Gun.Rotation);
        bullet.Position = Tip.GlobalPosition;
        GetParent().AddChild(bullet);
        ReadyToFire = false;
        ShootTimer.Start();
    }

    private void CooldownComplete()
    {
        ReadyToFire = true;
    }
}

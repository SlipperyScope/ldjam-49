using Godot;
using System;
using System.Collections.Generic;

public enum FiringPattern
{
    Parallel,
    V,
    Circle
}

public class FireController : Node2D
{
    const String GlobalDataPath = "/root/GlobalData";
    public GlobalData Global { get; private set; } 

    const String BulletScenePath = "res://Scenes/Bullet.tscn";
    const String TipPath = "Tip";

    public Node2D Tip { get; private set; }

    public TurretController Controller { get; private set; }

    /// <summary>
    /// Time since ready in seconds
    /// </summary>
    public Single Time { get; private set; } = 0f;

    /// <summary>
    /// Time (at/after which) next shot is possible
    /// </summary>
    public Single NextShotTime { get; private set; } = 1f;

    /// <summary>
    /// Number of shots in a burst
    /// </summary>
    public Int32 BurstSize { get; set; } = 3;
    
    /// <summary>
    /// Number of shots fired in the current burst
    /// </summary>
    private Int32 NumberInBurst = 0;

    /// <summary>
    /// Time between shots in a burst
    /// </summary>
    public DFloat BurstInterval { get; set; } = 0.1f;

    /// <summary>
    /// Time between bursts
    /// </summary>
    public DFloat BurstCooldown { get; set; } = 0.4f;

    /// <summary>
    /// Shape of bullet spawns
    /// </summary>
    public FiringPattern SpawnPattern;

    /// <summary>
    /// Instancable bullet Scene
    /// </summary>
    public PackedScene BulletScene;

    /// <summary>
    /// Bullet speed in u/s
    /// </summary>
    public Single BulletSpeed { get; set; } = 800f;

    /// <summary>
    /// True to be horny
    /// </summary>
    public Boolean DoHorn { get; set; } = false;

    /// <summary>
    /// Whether turret should fire
    /// </summary>
    public Boolean DoFire { get; set; } = false;

    public List<ProjectileDefinition> Projectiles;

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        Tip = GetNode<Node2D>(TipPath);
        BulletScene = GD.Load<PackedScene>(BulletScenePath);
        Controller = GetParent<TurretController>();
        Global = GetNode<GlobalData>(GlobalDataPath);
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        Time += delta;

        if (DoFire is true && Time > NextShotTime)
        {
            Fire();
            NumberInBurst++;
            if (NumberInBurst >= BurstSize)
            {
                NextShotTime = Time + BurstCooldown / 1000f;
                NumberInBurst = 0;
            }
            else
            {
                NextShotTime = Time + BurstInterval / 1000f;
            }
        }
    }

    private void Fire()
    {
        Global.Shots++;
        foreach (var definition in Projectiles)
        {
            SpawnProjectile(definition);
        }
        if (DoHorn is true)
        {
            GetParent().GetNode<AudioStreamPlayer>("SFX").Play();
        }
    }

    private void SpawnProjectile(ProjectileDefinition definition)
    {
        var projectile = BulletScene.Instance<Bullet>();
        GetTree().Root.AddChild(projectile);

        var radians = definition.initRotation;
        var rotation = (Single)radians + Tip.GlobalRotation;
        var direction = new Vector2((Single)Math.Cos(rotation), (Single)Math.Sin(rotation));

        projectile.Damage = definition.damage;
        projectile.Penetrates = definition.penetrating;
        projectile.Velocity =  direction * definition.speed * 200f;
        projectile.Position = Tip.GlobalPosition + definition.initPosition.Rotated(rotation);
        projectile.SpriteRotation = rotation;
        projectile.Scale = new Vector2(definition.scale, definition.scale);
        Controller.AddStability(-definition.cost);
        Global.Bullets++;
    }
}

using Godot;
using System;

public class EnemySpawner : Node
{
    #region Enemies
    const String SpiderPath = "res://Scenes/Enemies/SpiderEnemy.tscn";
    const String TruckPath = "res://Scenes/Enemies/TruckEnemy.tscn";
    const String CarPath = "res://Scenes/Enemies/CarEnemy.tscn";

    private PackedScene Spider;
    private PackedScene Truck;
    private PackedScene Car;
    #endregion

    [Export]
    private NodePath BackgroundPath;

    [Export]
    private NodePath TurretPath;
    private TurretController Turret;

    private Rect2 MapBounds;


    private Single Time = -1f;

    private Single NextSpawnTime;

    public Int32 Difficulty = 0;

    public override void _EnterTree()
    {
        Spider = GD.Load<PackedScene>(SpiderPath);
    }

    public override void _Ready()
    {
        MapBounds = GetNode<Sprite>(BackgroundPath).GetRect();
        Turret = GetNode<TurretController>(TurretPath);        
    }

    public override void _Process(Single delta)
    {
        Time += delta;

        if (Time >= NextSpawnTime)
        {
            Spawn();    
        }
    }

    private void Spawn()
    {
        var SpawnLocation = GetRandomOffMapLocation();
        switch (Difficulty)
        {
            default:
                for (Int32 i = (Int32)GD.Randi() % 2 + 3; i > 0; i--)
                {
                    SpawnSingle(Spider, SpawnLocation + GetSpawnAreaLocation());
                }

                NextSpawnTime = Time + 4f;
                break;
        }
    }

    private void SpawnSingle(PackedScene enemyScene, Vector2 location)
    {
        var enemy = enemyScene.Instance<SeaAnemone>();
        GetTree().Root.AddChild(enemy);
        enemy.Position = location;
        enemy.TargetLocation = Turret.GlobalPosition;
    }

    private Vector2 GetRandomOffMapLocation() => new Vector2(0f, 1f).Rotated((Single)GD.RandRange(-Math.PI, Math.PI)) * 2500f;

    private Vector2 GetSpawnAreaLocation() => new Vector2(0f, 1f).Rotated((Single)GD.RandRange(-Math.PI, Math.PI)) * (Single)GD.RandRange(0f, 1000f);
}

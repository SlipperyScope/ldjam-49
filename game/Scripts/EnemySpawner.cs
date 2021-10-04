using Godot;
using System;

public class EnemySpawner : Node
{
    const String GlobalDataPath = "/root/GlobalData";
    public GlobalData Global { get; private set; }

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
        Car = GD.Load<PackedScene>(CarPath);
    }

    public override void _Ready()
    {
        MapBounds = GetNode<Sprite>(BackgroundPath).GetRect();
        Turret = GetNode<TurretController>(TurretPath);
        Global = GetNode<GlobalData>(GlobalDataPath);
        Global.KillIncreased += OnKillIncreased;
    }

    public override void _Process(Single delta)
    {
        Time += delta;

        if (Time >= NextSpawnTime)
        {
            Spawn();    
        }
    }

    private void OnKillIncreased(object sender, OnKillEventArgs e)
    {
        Difficulty = e.Kills / 10;
    }

    private void Spawn()
    {
        var SpawnLocation = GetRandomOffMapLocation();
        if (Difficulty < 5)
        {
            for (Int32 i = (Int32)GD.Randi() % 2 + Difficulty; i > 0; i--)
            {
                SpawnSingle(Spider, SpawnLocation + GetSpawnAreaLocation());
            }

            NextSpawnTime = Time + 4f;
        }
        else //if (Difficulty < 10)
        {
            for (Int32 i = (Int32)GD.Randi() % 2 + Difficulty / 2; i > 0; i--)
            {
                SpawnSingle(Spider, SpawnLocation + GetSpawnAreaLocation());
            }
            for (Int32 i = (Int32)GD.Randi() % 1 + Difficulty / 4; i > 0; i--)
            {
                SpawnSingle(Car, SpawnLocation + GetSpawnAreaLocation());
            }
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

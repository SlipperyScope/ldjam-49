using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class Director : Node
{
    public static Dictionary<string, string> E = new Dictionary<string, string>(){
        { "spider", "res://Scenes/Enemies/SpiderEnemy.tscn" },
        { "truck", "res://Scenes/Enemies/TruckEnemy.tscn" },
        { "car", "res://Scenes/Enemies/CarEnemy.tscn" },
    };
    public delegate void EnemyWaveHandler(object sender, Wave wave);
    public delegate void EnemySpawnHandler(object sender, PackedScene enemyType);
    public event EnemyWaveHandler EnemyWave;
    public event EnemySpawnHandler EnemySpawn;

    public List<PackedScene> UnlockedEnemies = new List<PackedScene>();

    const String GlobalDataPath = "/root/GlobalData";
    private GlobalData Global;
    private float time = 0;
    public float SpawnRate = 1; // N per second
    public float NextSpawn = 1;

    private Dictionary<string, bool> Unlocks = new Dictionary<string, bool>();

    public Dictionary<int, Wave> Waves = new Dictionary<int, Wave>(){
        { 5, new Wave(10, "spider") },
        { 20, new Wave(10, "spider", "truck") },
        { 50, new Wave(100, "spider", "truck", "car") },
    };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Global = GetNode<GlobalData>(GlobalDataPath);
        this.Global.KillIncreased += (object sender, OnKillEventArgs e) => {
            this.MakeEnemyWave(e.Kills);
        };

        this.UnlockedEnemies.Add(GD.Load<PackedScene>(Director.E["spider"]));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        this.time += delta;
        if (time > 30) this.IntroduceTruck();
        if (time > 120) this.IntroduceCar();

        if (time > this.NextSpawn) {
            this.Spawn();
            this.NextSpawn = this.time + this.ComputeSpawn(this.time);
        }

        // Old doodoo. Kept for posterity

        // var maxSpawnRate = 15f; // Per second
        // var initSpawnRate = 1f;
        // var secondsUntilMax = 180f;

        // // y = a / (1 + b*e^-kx ), k > 0
        // // This will start slow, speed up, and then slow down again
        // var k = 8f / secondsUntilMax;
        // var b = maxSpawnRate - initSpawnRate;
        // this.SpawnRate = (float)(maxSpawnRate / (1f + b * Math.Pow(Math.E, -k * this.time)));
    }

    private void Spawn() {
        int idx = (int)(GD.Randi() % this.UnlockedEnemies.Count);
        PackedScene type = this.UnlockedEnemies[idx];
        EnemySpawn?.Invoke(this, type);
    }

    private float ComputeSpawn(float time) {
        // y = a - b*e^0.5cx
        float a = 1.2f;
        float b = 0.1f;
        float c = 0.1f;

        float fastestSpawnRate = 0.1f; // Ten per second

        return Math.Max(fastestSpawnRate, (float)(a - b*Math.Pow(Math.E, 0.5*c*this.time)));
    }

    private void MakeEnemyWave(int kills) {
        if (this.Waves.ContainsKey(kills)) {
            this.EnemyWave?.Invoke(this, this.Waves[kills]);
        }
    }

    private void IntroduceTruck() {
        if (!this.Unlocks.ContainsKey("truck")) {
            this.UnlockedEnemies.Add(GD.Load<PackedScene>(Director.E["truck"]));
            this.Unlocks["truck"] = true;
        }
    }

    private void IntroduceCar() {
        if (!this.Unlocks.ContainsKey("car")) {
            this.UnlockedEnemies.Add(GD.Load<PackedScene>(Director.E["car"]));
            this.Unlocks["car"] = true;
        }
    }
}

public class Wave {
    public int Count;
    public List<string> EnemyMix = new List<string>();

    public Wave(int Count, params string[] Mix) {
        this.Count = Count;
        foreach (var enemy in Mix) {
            // Look up res location from enemy string
            this.EnemyMix.Add(Director.E[enemy]);
        }
    }
}

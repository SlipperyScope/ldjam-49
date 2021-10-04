using Godot;
using System;

public class StabilityBar : Node
{
    private Container bar;
    public float maxStability = 100;
    public float currentStability = 50;
    public TurretController turret;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.bar = GetNode<Container>("Bar");
        // GAAMMEE JAM!
        this.turret = GetNode<TurretController>("/root/Node2D/Turret");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        this.bar.RectScale = new Vector2(Math.Max(0, this.turret.CurrentStability / this.turret.MaxStability), 1f);
    }
}

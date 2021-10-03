using Godot;
using System;

public class StabilityBar : Node
{
    private Container bar;
    public float maxStability = 100;
    public float currentStability = 50;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.bar = GetNode<Container>("Bar");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        this.bar.RectScale = new Vector2(this.currentStability / this.maxStability, 1f);
    }
}

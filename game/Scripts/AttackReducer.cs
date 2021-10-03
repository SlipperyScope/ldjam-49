using Godot;
using System;

public class AttackReducer : Node
{
    private AttackDefinition start;
    public AttackDefinition end;
    public string[] augments;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.start = new AttackDefinition();
        this.start.projectiles.Add(new ProjectileDefinition());

        this.augments = new string[]{ "TripleShot", "KittenPurr" };

        GD.Print(this.start.Print());

        GD.Print("\n--------- ", String.Join(", ", this.augments), " ----------\n");

        this.Reduce();
        GD.Print(this.end.Print());

        this.augments = new string[]{ "TripleShot", "FiveShot", "RandoShot" };

        GD.Print("\n--------- ", String.Join(", ", this.augments), " ----------\n");

        this.Reduce();
        GD.Print(this.end.Print());
    }

    public void Reduce() {
        this.end = this.start.Clone();

        foreach(string augment in this.augments) {
            Augments.registry[augment].apply(this.end);
        }
    }
}

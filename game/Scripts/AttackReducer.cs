using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

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

        this.augments = new string[]{ "TripleShot", "FiveShot", "RandoShot", "Trey", "Quad" };

        GD.Print("\n--------- ", String.Join(", ", this.augments), " ----------\n");

        this.Reduce();
        GD.Print(this.end.Print());
    }

    public void Reduce() {
        this.end = this.start.Clone();

        // SpawnLine must always be included to get the default positioning logic
        var finalAugments = new List<string>(this.augments);
        finalAugments.Add("SpawnLine");

        // Get augmenters for each augment key
        var augmenters = new List<Augment>();
        foreach(string augment in finalAugments) {
            augmenters.Add(Augments.registry[augment]);
        }

        augmenters.Sort((x, y) => x.priority.CompareTo(y.priority));
        foreach(var augmenter in augmenters) {
            GD.Print(augmenter.priority);
            augmenter.apply(this.end);
        }
    }
}

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class AttackReducer : Node
{
    private Boolean DoDebug = false;

    public delegate void AttackDefinitionUpdatedHandler(object sender, AttackDefinitionUpdatedArgs e);
    public event AttackDefinitionUpdatedHandler AttackDefinitionUpdated;

    private AttackDefinition start;
    public AttackDefinition end;
    public List<string> augments = new List<string>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.start = new AttackDefinition();
        this.start.projectiles.Add(new ProjectileDefinition());

        // Make sure there is always a valid end definition
        this.Reduce();
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
            if (DoDebug) GD.Print(augmenter.priority);
            augmenter.apply(this.end);
        }

        if (DoDebug) GD.Print("--------- Reduced!");
        if (DoDebug) GD.Print(this.end.Print());

        AttackDefinitionUpdated?.Invoke(this, new AttackDefinitionUpdatedArgs(end.Clone()));
    }

    
}
public class AttackDefinitionUpdatedArgs : EventArgs
{
    public AttackDefinition Definition { get; private set; } 
    public AttackDefinitionUpdatedArgs(AttackDefinition definition)
    {
        Definition = definition;
    }
}
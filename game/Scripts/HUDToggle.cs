using Godot;
using System;

public class HUDToggle : TextureRect
{
    [Export]
    public string augment = "";
    [Export]
    public bool enabled = false;
    private AttackReducer attackReducer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        try {
            // this.attackReducer = GetParent().GetParent().GetNode<AttackReducer>("AttackReducer");
            this.attackReducer = GetNode<AttackReducer>("/root/Node2D/AttackReducer");
        } catch {
            GD.PrintErr("Couldn't find the AttackReducer node, continuing anyway");
        }
    }

    public void _on_TextureRect_gui_input(InputEvent evt) {
        if (evt is InputEventMouseButton ievt) {
            if (ievt.Pressed) {
                this.enabled = !this.enabled;
                this.Modulate = this.enabled ? new Color(0.8f, 0.8f, 1) : new Color(1, 1, 1);
                if (this.attackReducer != null) {
                    if (this.enabled) {
                        GD.Print($"Enabled {this.augment}");
                        this.attackReducer.augments.Add(this.augment);
                    } else {
                        GD.Print($"Disabled {this.augment}");
                        this.attackReducer.augments.Remove(this.augment);
                    }
                    this.attackReducer.Reduce();
                }
            }
        }
    }
}

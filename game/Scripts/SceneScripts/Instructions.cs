using Godot;
using System;

public class Instructions : Node2D
{
    const String BackPath = "Back";

    public Button Back { get; private set; }

    public override void _Ready()
    {
        Back = GetNode<Button>(BackPath);
        Back.Connect("pressed", this, nameof(OnBackPressed));
    }

    private void OnBackPressed()
    {
        GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
    }
}

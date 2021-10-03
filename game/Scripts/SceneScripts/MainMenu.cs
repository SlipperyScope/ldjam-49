using Godot;
using System;

public class MainMenu : Node2D
{
    const String PlayScene = "res://Scenes/Game.tscn";
    const String InstructionsScene = "res://Scenes/Instructions.tscn";
    const String CreditsScene = "res://Scenes/Credits.tscn";

    const String PlayPath = "Play";
    const String InstructionsPath = "Instructions";
    const String CreditsPath = "Credits";
    const String ExitPath = "Exit";

    public Button Play { get; private set; }
    public Button Instructions { get; private set; }
    public Button Credits { get; private set; }
    public Button Exit { get; private set; } 

    public override void _Ready()
    {
        Play = GetNode<Button>(PlayPath);
        Instructions = GetNode<Button>(InstructionsPath);
        Credits = GetNode<Button>(CreditsPath);
        Exit = GetNode<Button>(ExitPath);

        Play.Connect("pressed", this, nameof(OnPlayPressed));
        Instructions.Connect("pressed", this, nameof(OnInstructionsPressed));
        Credits.Connect("pressed", this, nameof(OnCreditsPressed));
        Exit.Connect("pressed", this, nameof(OnExitPressed));
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeScene(PlayScene);
    }

    private void OnInstructionsPressed()
    {
        GetTree().ChangeScene(InstructionsScene);
    }

    private void OnCreditsPressed()
    {
        GetTree().ChangeScene(CreditsScene);
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}

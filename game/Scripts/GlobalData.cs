using Godot;
using System;

public class GlobalData : Node
{
    public Int32 Kills { get; set; } = 0;
    public Int32 Shots { get; set; } = 0;
    public Int32 Bullets { get; set; } = 0;
    public Int32 Hits { get; set; } = 0;
    public Int32 EnemyHits { get; set; } = 0;
}

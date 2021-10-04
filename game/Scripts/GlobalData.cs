using Godot;
using System;

public class GlobalData : Node
{
    public delegate void OnKillHandler(object sender, OnKillEventArgs e);
    public event OnKillHandler KillIncreased;

    public Int32 Kills
    {
        get => _Kills;
        set
        {
            _Kills = value;
            KillIncreased?.Invoke(this, new OnKillEventArgs(_Kills));
        }
    }
    private Int32 _Kills = 0;

    public Int32 Shots { get; set; } = 0;
    public Int32 Bullets { get; set; } = 0;
    public Int32 Hits { get; set; } = 0;
    public Int32 EnemyHits { get; set; } = 0;
}

public class OnKillEventArgs : EventArgs
{
    public Int32 Kills;
    public OnKillEventArgs(Int32 kills)
    {
        Kills = kills;
    }
}

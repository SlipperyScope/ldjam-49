using Godot;
using System;

public class Turret : Node2D
{
    const Boolean SlowMo = false;
    /// <summary>
    /// Speed of turret rotation in angles per second
    /// </summary>
    [Export]
    public Single RotationSpeed = 20f;

    public Node2D Base { get; private set; }
    public Node2D Gun { get; private set; }
    private Label Output;

    private Int32 debugCount = 0;

    /// <summary>
    /// Ready
    /// </summary>
    public override void _Ready()
    {
        Base = GetNode<Node2D>("Base");
        Gun = GetNode<Node2D>("Gun");
        Output = GetNode<Label>("/root/Andrew/Label");
    }

    /// <summary>
    /// Process
    /// </summary>
    public override void _Process(Single delta)
    {
        if (SlowMo && debugCount++ % 30 != 0) return;

        Aim(delta);
        Fire();
    }

    private void Aim(Single delta, Boolean LimitRotationSpeed = true)
    {
        var target = GetTarget();

        if (LimitRotationSpeed is true)
        {
            var angle = Gun.GetAngleTo(target);
            var frameRotation = RotationSpeed * delta;
            //Output.Text = $"Angle: {angle} frameSpeed: {frameRotation} Willhit: {Math.Abs(angle) < frameRotation} gunRot: {Gun.Rotation} nextRot: {(angle < 0f ? frameRotation : -frameRotation)}";
            if (Math.Abs(angle) < frameRotation)
            {
                Gun.LookAt(target);
            }
            else
            {
                Gun.Rotation -= angle < 0f ? frameRotation : -frameRotation;
            }
        }
        else
        {
            Gun.LookAt(target);
        }
    }

    private Vector2 GetTarget()
    {
        return GetGlobalMousePosition();
    }

    private void Fire()
    {

    }
}

using Godot;
using System;
using System.Collections.Generic;

public class Camera : Node2D
{
    const String MapBlipPath = "res://Scenes/MapBlip.tscn";
    private PackedScene MapBlipScene;

    Camera2D camera;
    TextureRect minimap;
    Control gameoverScreen;
    Sprite map;
    TurretController turret;
    bool isDragging = false;
    private Timer MapUpdateTimer = new Timer();
    private List<Sprite> Blips = new List<Sprite>();



    public override void _EnterTree()
    {
        MapBlipScene = GD.Load<PackedScene>(MapBlipPath);
    }

    public override void _Ready()
    {
        MapUpdateTimer.Autostart = true;
        MapUpdateTimer.OneShot = false;
        MapUpdateTimer.WaitTime = 1f;
        MapUpdateTimer.Connect("timeout", this, nameof(UpdateMap));
        AddChild(MapUpdateTimer);

        camera = GetNode<Camera2D>("Camera2D");
        minimap = GetNode<TextureRect>("HUD/Minimap");
        map = GetParent().GetNode<Sprite>("MapSprite");
        gameoverScreen = GetNode<Control>("HUD/GameOver");
        turret = GetParent().GetNode<TurretController>("Turret");

        turret.DidDed += Gameover;

        setCameraLimit(map);
    }

    private void Gameover(object sender) {
        this.gameoverScreen.Visible = true;
    }

    // Signals are doodoo poopy ass and this only works because this signal is turned on in the editor
    public void _on_Minimap_gui_input(InputEvent evt) {
        if (evt is InputEventMouseButton ievt) {
            moveToMinimapPosition(ievt.Position);
            if (ievt.Pressed && !isDragging) isDragging = true;
            if (!ievt.Pressed && isDragging) isDragging = false;
        }
        if (evt is InputEventMouseMotion mevt) {
            if (isDragging) {
                moveToMinimapPosition(mevt.Position);
            }
        }
    }

    public void _on_GameOver_gui_input(InputEvent evt) {
        if (evt is InputEventMouseButton ievt) {
            gameoverScreen.Visible = false;
            foreach (Node target in GetTree().GetNodesInGroup("Targetable")) {
                target.QueueFree();
            }
            foreach (Node projectile in GetTree().GetNodesInGroup("Projectiles")) {
                projectile.QueueFree();
            }
            GetTree().ReloadCurrentScene();
        }
    }

    public void moveToMinimapPosition(Vector2 position) {
        // Translate position within sprite to position within map
        var scaleFactor = position / minimap.GetRect().Size;
        this.Position = map.GetRect().Size * scaleFactor - map.GetRect().Size * new Vector2(0.5f, 0.5f);
    }

    public void setCameraLimit(Sprite bg) {
        var rect = bg.GetRect();
        var width = (int)(rect.Size.x * bg.Scale.x);
        var height = (int)(rect.Size.y * bg.Scale.y);
        camera.LimitLeft = -width/2;
        camera.LimitTop = -height/2;
        camera.LimitRight = width/2;
        camera.LimitBottom = height/2;

        // Scale the minimap in proportion to the map
        // var widthFactor = rect.Size.x / rect.Size.y;
        // minimap.RectScale = new Vector2(widthFactor * 0.2f, 0.2f);

        // GAME JAM -- position the minimap so it's always in viewport
        var dimensions = new Vector2(1920f, 1080f);
        var margins = new Vector2(25f, 25f);
        var scaledSize = new Vector2(minimap.RectSize.x * minimap.RectScale.x, minimap.RectSize.y * minimap.RectScale.y);
        minimap.RectPosition = dimensions - margins - scaledSize;
    }

    private void UpdateMap()
    {
        var mapables = GetTree().GetNodesInGroup("Mapable");
        Blips.ForEach(b => b.QueueFree());
        Blips.Clear();
        foreach (Node2D mapable in mapables)
        {
            var blip = MapBlipScene.Instance<Sprite>();
            blip.Position = ((turret.GlobalPosition - mapable.GlobalPosition) / -10f + minimap.RectGlobalPosition / 4f) * minimap.RectScale + new Vector2(48f, 32f);
            Blips.Add(blip);
            minimap.AddChild(blip);
            GD.Print($"blip at {blip.GlobalPosition} map at {minimap.RectGlobalPosition}");
        }
    }
}

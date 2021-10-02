using Godot;
using System;

public class Camera : Node2D
{
    Camera2D camera;
    TextureRect minimap;
    Sprite map;
    bool isDragging = false;

    public override void _Ready()
    {
        camera = GetNode<Camera2D>("Camera2D");
        minimap = GetNode<TextureRect>("HUD/Minimap");
        map = GetParent().GetNode<Sprite>("MapSprite");

        setCameraLimit(map);
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
                GD.Print("motion", mevt.Position);
                moveToMinimapPosition(mevt.Position);
            }
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
    }
}

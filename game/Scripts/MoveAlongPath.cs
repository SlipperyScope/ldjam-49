// extends PathFollow2D
// import GDScript
//

//
// # Called every frame. 'delta' is the elapsed time since the previous frame.
// func _process(delta):
// 	
// 	GD.Print(get_offset() + moveSpeed * delta,get_offset() + moveSpeed * delta+5)
// 	
// 	
// 	set_offset(get_offset() + moveSpeed * delta)




using Godot;
using System;

public class MoveAlongPath : Godot.PathFollow2D
{
	public int moveSpeed = 200;
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
 //        
	// }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {

	  SetOffset(GetOffset() + moveSpeed * delta);
		// GD.set_offset(get_offset() + moveSpeed * delta);
  }
}

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProjectileDefinition : Node
{
  public float damage = 5;
  public float speed = 3;
  public DFloat initRotation = new DFloat(0);
  public Vector2 initPosition = new Vector2(0, 0);
  public float scale = 1;
  public bool penetrating = false;

  public ProjectileDefinition Clone() {
    return new ProjectileDefinition(){
      damage = this.damage,
      speed = this.speed,
      initRotation = new DFloat(this.initRotation.min, this.initRotation.max),
      initPosition = this.initPosition,
      scale = this.scale,
      penetrating = this.penetrating,
    };
  }

  public string Print() {
    return $"Damage: {damage}, Speed: {speed}, InitRotation: {initRotation}, InitPosition [{initPosition.x},{initPosition.y}]";
  }
}

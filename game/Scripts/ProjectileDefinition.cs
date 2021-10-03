using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProjectileDefinition : Node
{
  public float damage = 5;
  public float speed = 3;
  public float initRotation = 0;

  public ProjectileDefinition Clone() {
    return new ProjectileDefinition(){
      damage = this.damage,
      speed = this.speed,
      initRotation = this.initRotation,
    };
  }

  public string Print() {
    return $"Damage: {damage}, Speed: {speed}, InitRotation: {initRotation}";
  }
}

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
    return $"Damage: {damage}, Speed: {speed}, Scale: {scale}, Penetrating: {penetrating}, InitRotation: {initRotation}, InitPosition [{initPosition.x},{initPosition.y}]";
  }

  public float cost {
    get {
      // Base cost should be 1;
      var speedF = 3 / 10;
      var damageF = 5 / 10;
      var scaleF = 2 / 10;

      var total = this.speed * speedF + this.damage * damageF + this.scale * scaleF;
      if (this.penetrating) total *= 1.3f;

      // In case of emergency, break glass
      var justMakeTheNumberBigger = 1;
      total *= justMakeTheNumberBigger;

      return total;
    }
  }
}

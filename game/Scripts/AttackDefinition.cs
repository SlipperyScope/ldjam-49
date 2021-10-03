using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
public delegate void ProjectileApplier(ProjectileDefinition definition);

public class AttackDefinition : Node
{
  public DFloat shotDelay = new DFloat(250); // milliseconds, I guess?
  public DFloat burstDelay = new DFloat(50);
  public int burstCount = 1;
  public bool canHasHorn = false;
  public List<ProjectileDefinition> projectiles = new List<ProjectileDefinition>();

  public AttackDefinition Clone() {
    var clone = new AttackDefinition(){
      shotDelay = this.shotDelay,
      burstDelay = this.burstDelay,
      burstCount = this.burstCount,
      canHasHorn = this.canHasHorn,
    };

    this.projectiles.ForEach(p => {
      clone.projectiles.Add(p.Clone());
    });

    return clone;
  }

  public void EachProjectile(ProjectileApplier apply) {
    foreach (var projectile in this.projectiles) {
      apply(projectile);
    }
  }

  public string Print() {
    var lines = new List<string>(){
      $"ShotDelay: {shotDelay}, BurstDelay: {burstDelay}, BurstCount: {burstCount}",
      $"Horn? {(canHasHorn ? "Yes" : "No")}",
      "Projectiles:",
    };

    for (int idx = 0; idx < this.projectiles.Count; idx++) {
      lines.Add($"  {idx+1}) {projectiles[idx].Print()}");
    }

    return String.Join("\n", lines);
  }
}

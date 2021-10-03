using Godot;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class Augments {
  //
  // Shot speed & bursts
  //
  public static Augment TripleShot = new Augment(50, def => def.burstCount *= 3);
  public static Augment FiveShot = new Augment(50, def => def.burstCount *= 5);
  public static Augment KittenPurr = new Augment(50, def => def.shotDelay = new DFloat(def.shotDelay.min / 4f, def.shotDelay.max / 4f));
  public static Augment RandoShot = new Augment(50, def => def.shotDelay = new DFloat(25, 600));

  //
  // Projectile counts
  //
  public static Augment Deuce = new Augment(60, def => Augments.MakeProjectiles(def, 1));
  public static Augment Trey = new Augment(60, def => Augments.MakeProjectiles(def, 2));
  public static Augment Quad = new Augment(60, def => Augments.MakeProjectiles(def, 3));
  public static Augment Porcupine = new Augment(60, def => Augments.MakeProjectiles(def, 10));

  //
  // Spawn patterns
  //
  public static Augment SpawnLine = new Augment(65, def => {
    var spacing = 5f;
    if (def.projectiles.Count > 1) {
      var range = (def.projectiles.Count - 1) * spacing;
      var start = -range/2;
      for (int idx = 0; idx < def.projectiles.Count; idx++) {
        def.projectiles[idx].initPosition = new Vector2(0, start + spacing * idx);
      }
    }
  });

  //
  // The one true augment
  //
  public static Augment Horn = new Augment(101, def => def.canHasHorn = true);

  private static void MakeProjectiles(AttackDefinition definition, int count) {
    for (int i = 0; i < count; i++) {
      definition.projectiles.Add(new ProjectileDefinition());
    }
  }

  public static Dictionary<string, Augment> registry {
    get {
      var dict = new Dictionary<string, Augment>();
      var metaAugments = typeof(Augments);
      var fields = metaAugments.GetFields();
      foreach(var field in fields) {
        if (field.Name != "registry") {
          dict.Add(field.Name, field.GetValue(metaAugments) as Augment); // Yolo?
        }
      }
      return dict;
    }
  }
}

public delegate void Applier(AttackDefinition definition);

public class Augment {
  public int priority = 50; // Sorted high to low, maybe, idk
  public Applier apply;

  public Augment(int priority, Applier apply) {
    this.priority = priority;
    this.apply = apply;
  }
}

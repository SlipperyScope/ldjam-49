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
    if (def.projectiles.Count > 1) {
      var spacing = 5f;
      var range = (def.projectiles.Count - 1) * spacing;
      var start = -range/2;
      for (int idx = 0; idx < def.projectiles.Count; idx++) {
        def.projectiles[idx].initPosition = new Vector2(0, start + spacing * idx);
      }
    }
  });

  public static Augment SpawnV = new Augment(70, def => {
    if (def.projectiles.Count > 1) {
      var angleRange = Math.PI / 2 * def.projectiles.Count / 5;
      var angleStep = angleRange / def.projectiles.Count;

      // Lol, no clue if this works. We'll see later I guess
      for (int idx = 0; idx < def.projectiles.Count; idx++) {
        def.projectiles[idx].initPosition = new Vector2(0, 0);
        def.projectiles[idx].initRotation = new DFloat(-angleRange/2 + angleStep * idx);
      }
    }
  });

  public static Augment SpawnCircle = new Augment(70, def => {
    if (def.projectiles.Count > 1) {
      var angleRange = Math.PI / 2;
      var angleStep = angleRange / def.projectiles.Count;

      for (int idx = 0; idx < def.projectiles.Count; idx++) {
        def.projectiles[idx].initPosition = new Vector2(0, 0);
        def.projectiles[idx].initRotation = new DFloat(angleStep * idx);
      }
    }
  });

  public static Augment SpawnRandom = new Augment(70, def => {
    for (int idx = 0; idx < def.projectiles.Count; idx++) {
      def.projectiles[idx].initRotation = new DFloat(0, Math.PI * 2);
    }
  });

  //
  // The one true augment
  //
  public static Augment Horn = new Augment(101, def => def.canHasHorn = true);

  //
  // Physical Attributes
  //
  public static Augment Big = new Augment(80, def => def.EachProjectile(p => p.scale *= 2));
  public static Augment Fast = new Augment(80, def => def.EachProjectile(p => p.speed *= 3));
  public static Augment Slow = new Augment(80, def => def.EachProjectile(p => p.speed /= 3));
  public static Augment Penetrating = new Augment(80, def => def.EachProjectile(p => p.penetrating = true));

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

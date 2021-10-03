using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class Augments {
  public static Augment TripleShot = new Augment(50, def => def.burstCount *= 3);
  public static Augment FiveShot = new Augment(50, def => def.burstCount *= 5);
  public static Augment KittenPurr = new Augment(50, def => def.shotDelay = new DFloat(def.shotDelay.min / 4f, def.shotDelay.max / 4f));
  public static Augment RandoShot = new Augment(50, def => def.shotDelay = new DFloat(25, 600));

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

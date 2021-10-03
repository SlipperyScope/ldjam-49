using Godot;
using System;

public class DFloat {
  public readonly double min;
  public readonly double max;



  public DFloat(double val) {
    this.min = val;
    this.max = val;
  }

  public DFloat(double min, double max) {
    this.min = min;
    this.max = max;
  }

  public double Yield() {
    return GD.RandRange(this.min, this.max);
  }

  public override string ToString() {
    return $"({this.min} to {this.max})";
  }
}

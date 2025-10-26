namespace Chickensoft.Log.Godot.Tests;

using System.Reflection;
using Chickensoft.GoDotTest;
using global::Godot;

public partial class Tests : Node2D
{
  public override void _Ready() => CallDeferred(MethodName.RunTests);

  public void RunTests() =>
    GoTest.RunTests(Assembly.GetExecutingAssembly(), this);
}

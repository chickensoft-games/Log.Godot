namespace Chickensoft.Log.Godot;

using System.Diagnostics.CodeAnalysis;
using global::Godot;

/// <summary>
/// An <see cref="ILogWriter"/> that outputs to Godot's console using
/// <see cref="GD.Print(string)"/>, <see cref="GD.PushWarning(string)"/>, and
/// <see cref="GD.PushError(string)"/>.
/// </summary>
/// <remarks>
/// <para>
/// If used in the same log with <see cref="TraceWriter"/>, this will cause
/// doubled output in Godot's console, as Godot already captures output from
/// <see cref="System.Diagnostics.Trace"/>.
/// </para>
/// <para>
/// Warnings and errors also print the message since
/// <see cref="GD.PushWarning(string)"/> and <see cref="GD.PushError(string)"/>
/// don't always show up in the output when debugging.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Godot output is untestable")]
public sealed class GDWriter : ILogWriter
{
  /// <inheritdoc/>
  public void WriteError(string message)
  {
    GD.PushError(message);
    WriteMessage(message);
  }

  /// <inheritdoc/>
  public void WriteMessage(string message) => GD.Print(message);

  /// <inheritdoc/>
  public void WriteWarning(string message)
  {
    GD.PushWarning(message);
    WriteMessage(message);
  }
}

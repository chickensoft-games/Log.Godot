# 🪵 Log.Godot

[![Chickensoft Badge][chickensoft-badge]][chickensoft-website] [![Discord][discord-badge]][discord] [![Read the docs][read-the-docs-badge]][docs] ![line coverage][line-coverage] ![branch coverage][branch-coverage]

Opinionated logging for C# in Godot, based on [Chickensoft.Log][chickensoft-log-gh].

---

<p align="center">
<img alt="Chickensoft.Log.Godot" src="Chickensoft.Log.Godot/icon.png" width="200">
</p>

## 📦 Installation

> [!TIP]
> For logging in pure C# without Godot, see [Chickensoft.Log][chickensoft-log-gh]. Note that the `TraceWriter` from
> Chickensoft.Log will produce output in Godot's console.

Install the latest version of the [Chickensoft.Log.Godot] and [Chickensoft.Log] packages from nuget:

```xml
dotnet add package Chickensoft.Log
dotnet add package Chickensoft.Log.Godot
```

## 🌱 Usage

### Essentials

For an overview of the logging system, see [Chickensoft.Log][chickensoft-log-gh]. This package provides Chickensoft.Log-compatible writers for output to the Godot debug console and Godot file paths.

> [!WARNING]
> If you are using `TraceWriter` from the Chickensoft.Log package, you *probably should **not*** also use a `GDWriter` for output to the Godot debug console in the same log! Godot uses a custom `TraceListener` to pick up .NET messages directed through `Trace`, so any messages sent to a `TraceWriter` will already be directed to the Godot console when run in Godot. Using `GDWriter` will only create doubled output.

### Setup

```csharp
public class MyClass
{
  // Create a log with the name of MyClass, outputting to the Godot debug console
  private ILog _log = new Log(nameof(MyClass), new GDWriter());
}
```

### Logging

```csharp
public void MyMethod()
{
  // Outputs "Info (MyClass): A log message"
    _log.Print("A log message");
    // Outputs "Warn (MyClass): A warning message"
    _log.Warn("A warning message");
    // Outputs "Error (MyClass): An error occurred"
    _log.Err("An error occurred");

    try
    {
      SomethingThatThrows();
    }
    catch (Exception e)
    {
      // Outputs the value of e.ToString(), prefixed by a line labeling it an
      // exception, as an error
      _log.Print(e);
    }

    // Outputs the current stack trace as a standard log message
    _log.Print(new System.Diagnostics.StackTrace());
}
```

> [!TIP]
> For details on formatting log messages, see [Chickensoft.Log][chickensoft-log-gh].

## ✒️ Writer Types

The Chickensoft.Log.Godot package provides two writer types for use with Godot:

* `GDWriter`: Outputs log messages to the Godot console.
* `GDFileWriter`: Outputs log messages to file using Godot's file I/O system, to
support writing files to Godot's `"res://"` and `"user://"` paths. By default,
`GDFileWriter` will write to `"user://output.log"`, but you can either configure a
different default, or configure individual `GDFileWriter`s to write to
particular files on creation. To avoid concurrency issues, `GDFileWriter` is
implemented as a pseudo-singleton with a single instance per file name.

### Using `GDFileWriter`

Create a log that outputs messages to the default filename `"user://output.log"`:

```csharp
public class MyClass
{
  private ILog _log = new Log(nameof(MyClass), GDFileWriter.Instance());
}
```

---
Create a log that outputs messages to a custom filename:

```csharp
public class MyClass
{
  private ILog _log = new Log(nameof(MyClass),
    GDFileWriter.Instance("user://CustomFileName.log"));
}
```

---
Change the default filename for `GDFileWriter`s:

```csharp
public class Entry
{
  public static void Main()
  {
    // Change the default filename for GDFileWriter before any writers are created
    GDFileWriter.DefaultFileName = "user://MyDefaultFileName.log";
  }
}

public class MyClass
{
  private ILog _log = new Log(nameof(MyClass), GDFileWriter.Instance());
}
```

> [!WARNING]
> Changing the default value for the log file name will affect newly-created
> `GDFileWriter`s, but will not affect ones that already exist.

## 💁 Getting Help

*Having issues?* We'll be happy to help you in the [Chickensoft Discord server][discord].

---

🐣 Package generated from a 🐤 Chickensoft Template — <https://chickensoft.games>

[chickensoft-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/chickensoft_badge.svg
[chickensoft-website]: https://chickensoft.games
[discord-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/discord_badge.svg
[discord]: https://discord.gg/gSjaPgMmYW
[read-the-docs-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/read_the_docs_badge.svg
[docs]: https://chickensoft.games/docsickensoft%20Discord-%237289DA.svg?style=flat&logo=discord&logoColor=white
[line-coverage]: Chickensoft.Log.Godot.Tests/badges/line_coverage.svg
[branch-coverage]: Chickensoft.Log.Godot.Tests/badges/branch_coverage.svg

[Chickensoft.Log]: https://www.nuget.org/packages/Chickensoft.Log
[Chickensoft.Log.Godot]: https://www.nuget.org/packages/Chickensoft.Log.Godot
[chickensoft-log-gh]: https://github.com/chickensoft-games/Log

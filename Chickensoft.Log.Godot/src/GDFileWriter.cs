namespace Chickensoft.Log.Godot;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using global::Godot;

/// <summary>
/// An <see cref="ILogWriter"/> that directs output to a file via Godot's I/O.
/// </summary>
public sealed class GDFileWriter : ILogWriter {
  [ExcludeFromCodeCoverage(Justification = "Godot file I/O is untestable")]
  internal static void GDAppendFileText(string path, string text) {
    using var file = FileAccess.Open(path, FileAccess.ModeFlags.ReadWrite);
    file.SeekEnd();
    file.StoreLine(text);
  }

  [ExcludeFromCodeCoverage(Justification = "Godot file I/O is untestable")]
  internal static void GDCreateFile(string path) {
    using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
  }

  internal delegate void AppendTextDelegate(string path, string text);
  internal static AppendTextDelegate AppendTextDefault { get; } =
    GDAppendFileText;
  internal static AppendTextDelegate AppendText { get; set; } =
    AppendTextDefault;

  internal delegate void CreateFileDelegate(string path);
  internal static CreateFileDelegate CreateFileDefault { get; } = GDCreateFile;
  internal static CreateFileDelegate CreateFile { get; set; } =
    CreateFileDefault;

  // protect static members from simultaneous thread access
  private static readonly object _singletonLock = new();

  // Implemented as a pseudo-singleton to enforce one truncation per file per
  // execution
  private static readonly Dictionary<string, GDFileWriter> _instances = [];

  /// <summary>The default filename for logs.</summary>
  public const string DEFAULT_FILE_NAME = "output.log";

#pragma warning disable IDE0032 // Use auto property
  private static string _defaultFileName = DEFAULT_FILE_NAME;
#pragma warning restore IDE0032 // Use auto property

  /// <summary>
  /// The default file name that will be used when creating a
  /// <see cref="GDFileWriter"/> if no filename is specified. Defaults to
  /// "user://output.log".
  /// </summary>
  /// <remarks>
  /// This default may be changed. If it is changed after a default
  /// <see cref="GDFileWriter"/> has already been created, any future calls to
  /// <see cref="Instance()"/> will return a different <see cref="GDFileWriter"/>
  /// outputting to the new default, but previously-created instances will not
  /// be changed and will continue outputting to the original default file.
  /// </remarks>
  public static string DefaultFileName {
    get {
      lock (_singletonLock) {
        return _defaultFileName;
      }
    }
    set {
      lock (_singletonLock) {
        _defaultFileName = value;
      }
    }
  }

  /// <summary>
  /// Obtains a <see cref="GDFileWriter"/> directing output to the given filename.
  /// </summary>
  /// <param name="fileName">
  /// The filename to which output should be directed when using the returned
  /// <see cref="GDFileWriter"/>.
  /// </param>
  /// <returns>
  /// A <see cref="GDFileWriter"/> outputting to a file at
  /// <paramref name="fileName"/>.
  /// </returns>
  /// <remarks>
  /// If a <see cref="GDFileWriter"/> outputting to <paramref name="fileName"/>
  /// already exists, a reference to the same <see cref="GDFileWriter"/> will be
  /// returned. If not, a new <see cref="GDFileWriter"/> will be created. When a
  /// new <see cref="GDFileWriter"/> is created, if the file at
  /// <paramref name="fileName"/> already exists, it will erased; if not, it
  /// will be created.
  /// </remarks>
  public static GDFileWriter Instance(string fileName) {
    lock (_singletonLock) {
      if (_instances.TryGetValue(fileName, out var writer)) {
        return writer;
      }
      writer = new GDFileWriter(fileName);
      _instances[fileName] = writer;
      return writer;
    }
  }

  /// <summary>
  /// Obtains a <see cref="GDFileWriter"/> that directs output to the current
  /// <see cref="DefaultFileName"/>.
  /// </summary>
  /// <returns>
  /// A <see cref="GDFileWriter"/> outputting to a file at
  /// <see cref="DefaultFileName"/>.
  /// </returns>
  /// <seealso cref="Instance(string)"/>
  /// <seealso cref="DefaultFileName"/>
  public static GDFileWriter Instance() {
    lock (_singletonLock) {
      return Instance(DefaultFileName);
    }
  }

  /// <summary>
  /// Remove a <see cref="GDFileWriter"/> that had previously been created.
  /// While not necessary, this can free up resources if writing to many
  /// different log files.
  /// </summary>
  /// <param name="fileName">Filename for the log.</param>
  /// <returns>The file writer, if one existed for the given filename.
  /// Otherwise, just null.</returns>
  public static GDFileWriter? Remove(string fileName) {
    lock (_singletonLock) {
      if (_instances.TryGetValue(fileName, out var writer)) {
        _instances.Remove(fileName);
        return writer;
      }
    }
    return null;
  }

  private readonly object _writingLock = new();

  /// <summary>
  /// The path of the file this Writer is writing to.
  /// </summary>
  public string FileName { get; }

  private GDFileWriter(string fileName) {
    FileName = fileName;
    lock (_writingLock) {
      // Clear the file
      CreateFile(FileName);
    }
  }

  private void WriteLine(string message) {
    lock (_writingLock) {
      AppendText(FileName, message);
    }
  }

  /// <inheritdoc/>
  public void WriteError(string message) {
    WriteLine(message);
  }

  /// <inheritdoc/>
  public void WriteMessage(string message) {
    WriteLine(message);
  }

  /// <inheritdoc/>
  public void WriteWarning(string message) {
    WriteLine(message);
  }
}

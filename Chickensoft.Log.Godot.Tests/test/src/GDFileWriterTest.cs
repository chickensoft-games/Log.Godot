namespace Chickensoft.Log.Godot.Tests;

using System;
using System.Text;
using Chickensoft.GoDotTest;
using global::Godot;
using Shouldly;

public class GDFileWriterStreamTester : IDisposable
{
  private bool _isDisposed;
  private readonly StringBuilder _sb;

  public GDFileWriterStreamTester(
    string filename = GDFileWriter.DEFAULT_FILE_NAME
  )
  {
    _sb = new StringBuilder();

    GDFileWriter.AppendText = (_, message) => _sb.AppendLine(message);
    GDFileWriter.CreateFile = (_) => _sb.Clear();
  }

  public string GetString() => _sb.ToString();

  public void Dispose()
  {
    if (_isDisposed)
    { return; }

    GC.SuppressFinalize(this);
    _isDisposed = true;

    GDFileWriter.AppendText = GDFileWriter.AppendTextDefault;
    GDFileWriter.CreateFile = GDFileWriter.CreateFileDefault;
  }
}

public class GDFileWriterTest : TestClass
{
  public GDFileWriterTest(Node testScene) : base(testScene)
  {
  }

  [Test]
  public void DefaultFileName()
  {
    GDFileWriter.DefaultFileName.ShouldBe(GDFileWriter.DEFAULT_FILE_NAME);

    var filename = "test.log";
    GDFileWriter.DefaultFileName = filename;
    GDFileWriter.DefaultFileName.ShouldBe(filename);

    GDFileWriter.DefaultFileName = GDFileWriter.DEFAULT_FILE_NAME;
  }

  [Test]
  public void DefaultInstance()
  {
    using var tester = new GDFileWriterStreamTester();
    var writer = GDFileWriter.Instance();
    writer.ShouldNotBeNull();
    writer.ShouldBeOfType<GDFileWriter>();
  }

  [Test]
  public void NewInstance()
  {
    using var tester = new GDFileWriterStreamTester();
    var filename = "test.log";
    var writer = GDFileWriter.Instance(filename);
    writer.ShouldNotBeNull();
    writer.ShouldBeOfType<GDFileWriter>();
  }

  [Test]
  public void ReusesInstanceAndRemoves()
  {
    using var tester = new GDFileWriterStreamTester();
    var filename = "test.log";
    var writer1 = GDFileWriter.Instance(filename);
    var writer2 = GDFileWriter.Instance(filename);
    writer1.ShouldBeSameAs(writer2);

    GDFileWriter.Remove(filename).ShouldBeSameAs(writer1);
    GDFileWriter.Remove(filename).ShouldBeNull();
  }

  [Test]
  public void WriteMessage()
  {
    using var tester = new GDFileWriterStreamTester();

    var writer = GDFileWriter.Instance();
    var value = "test message";

    writer.WriteMessage(value);

    tester.GetString().ShouldBe(value + System.Environment.NewLine);
  }

  [Test]
  public void WriteWarning()
  {
    using var tester = new GDFileWriterStreamTester();

    var writer = GDFileWriter.Instance();
    var value = "test message";

    writer.WriteWarning(value);

    tester.GetString().ShouldBe(value + System.Environment.NewLine);
  }

  [Test]
  public void WriteError()
  {
    using var tester = new GDFileWriterStreamTester();

    var writer = GDFileWriter.Instance();
    var value = "test message";

    writer.WriteError(value);

    tester.GetString().ShouldBe(value + System.Environment.NewLine);
  }
}

using Spectre.Console;

namespace MediaRelay.Console;


[TestClass]
public class AnsiConsoleTests
{
    [TestMethod]
    [DataRow(ExceptionFormats.Default)]
    [DataRow(ExceptionFormats.ShortenPaths)]
    [DataRow(ExceptionFormats.ShortenTypes)]
    [DataRow(ExceptionFormats.ShortenMethods)]
    [DataRow(ExceptionFormats.ShowLinks)]
    [DataRow(ExceptionFormats.ShortenEverything)]
    [DataRow(ExceptionFormats.NoStackTrace)]
    public void WriteException_ShouldRenderWithDifferentFormats(ExceptionFormats formats)
    {
        try
        {
            Level1();
        }
        catch(Exception ex)
        {
            AnsiConsole.WriteException(ex, formats);
        }


        static void Level1() => Level2();
        static void Level2() => Level3();
        static void Level3() => Level4();
        static void Level4() => throw new InvalidOperationException("操作失败", new OperationCanceledException("任务取消"));
    }
}

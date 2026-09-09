using Microsoft.Extensions.Logging;
using Spectre.Console.Testing;

namespace MediaRelay.Console.Logging;


[TestClass]
public sealed class ConsoleLoggerTests
{
    [TestMethod]
    [DataRow(3)]
    [DataRow(5)]
    [DataRow(10)]
    public void LogError_Exception(int deep)
    {
        var console = new TestConsole();
        var logger = new ConsoleLogger("MediaRelay.Console.Logging", console);

        using var _ = logger.BeginScope("Deep", deep);

        var exception = CreateExceptionRecursive(deep);
        logger.LogError(exception, "Error");

        System.Console.Write(console.Output);
    }



    private Exception CreateExceptionRecursive(int deep)
    {
        if (deep == 1)
            return new Exception($"最内层异常 (深度: {deep})");

        var inner = CreateExceptionRecursive(deep - 1);
        return new Exception($"异常 (深度: {deep})", inner);
    }
}

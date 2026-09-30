using Spectre.Console;
using Spectre.Console.Testing;

namespace MediaRelay.Console;

[TestClass]
public class AnsiConsoleTests
{
    [TestMethod]
    public void Fuck()
    {
        var mockConsole = new TestConsole();

        mockConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new RemainingTimeColumn())
            .Start(ctx =>
            {
                var task = ctx.AddTask("Converting video", maxValue: 100);

                while (!ctx.IsFinished)
                {
                    task.Increment(1);
                    Thread.Sleep(100);
                }
            });

        TestContext.Write(mockConsole.Output);
    }

    public TestContext TestContext { get; set; }
}
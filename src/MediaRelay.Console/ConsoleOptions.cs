namespace MediaRelay.Console;

public sealed record class ConsoleOptions
{
    public string UnprocessedInputFile { get; set; } = "input.dat";
}

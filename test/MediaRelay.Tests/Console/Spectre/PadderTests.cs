using Spectre.Console;

namespace MediaRelay.Console.Spectre;


[TestClass]
public sealed class PadderTests
{
    [TestMethod]
    public void Write()
    {
        var text = new Markup("[bold blue]Important Message[/]");
        var padded = new Padder(text);

        AnsiConsole.Write(padded);
    }

    [TestMethod]
    public void Write_Padding()
    {
        var text = new Markup("[yellow]Status: Active[/]");

        var padded = new Padder(text, new Padding(3));

        AnsiConsole.Write(padded);
    }

    [TestMethod]
    public void Write_Padding_Horizontal_Vertical()
    {
        var text = new Markup("[green]Success![/]");

        // 4 spaces left/right, 2 lines top/bottom
        var padded = new Padder(text, new Padding(4, 2));

        AnsiConsole.Write(padded);
    }

    [TestMethod]
    public void Write_Padding_Left_Top_Right_Bottom()
    {
        var text = new Markup("[red]Error detected[/]");

        // Left: 2, Top: 1, Right: 6, Bottom: 1
        var padded = new Padder(text, new Padding(2, 1, 6, 1));

        AnsiConsole.Write(padded);
    }
}

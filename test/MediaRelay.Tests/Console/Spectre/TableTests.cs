using Spectre.Console;

namespace MediaRelay.Console.Spectre;


[TestClass]
public sealed class TableTests
{
    [TestMethod]
    public void Write()
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);

        table.AddColumn("Name");
        table.AddColumn("Department");
        table.AddColumn("Sales");

        table.AddRow("Alice", "North", "$12,400");
        table.AddRow("Bob", "South", "$8,750");
        table.AddRow("Carol", "West", "$15,200");

        AnsiConsole.Write(table);
    }

    [TestMethod]
    public void Write_RoundedBorder_Aligned()
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey);

        table.AddColumn("Name");
        table.AddColumn("Department", col => col.Centered());
        table.AddColumn("Sales", col => col.RightAligned());

        table.AddRow("Alice", "North", "$12,400");
        table.AddRow("Bob", "South", "$8,750");
        table.AddRow("Carol", "West", "$15,200");

        AnsiConsole.Write(table);
    }

    [TestMethod]
    public void Write_RoundedBorder_Aligned_Title_Footer()
    {
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Title("[yellow bold]Q4 Sales Report[/]");

        table.AddColumn("Name");
        table.AddColumn("Department", col => col.Centered());
        table.AddColumn("Sales", col => col.RightAligned());

        table.AddRow("Alice", "North", "$12,400");
        table.AddRow("Bob", "South", "$8,750");
        table.AddRow("Carol", "West", "$15,200");

        // Add footer with totals
        table.Columns[0].Footer = new Text("Total", new Style(decoration: Decoration.Bold));
        table.Columns[1].Footer = new Text("");
        table.Columns[2].Footer = new Text("$36,350", new Style(Color.Green, decoration: Decoration.Bold));

        AnsiConsole.Write(table);
    }
}

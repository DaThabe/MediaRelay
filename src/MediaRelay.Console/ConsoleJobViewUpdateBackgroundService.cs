using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace MediaRelay.Console;


internal sealed class ConsoleJobViewUpdateBackgroundService : BackgroundService
{
    private readonly RelayInfoTable _table = new(AnsiConsole.Console);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _table.AddRow();
        _table.AddRow();
        _table.AddRow();


        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
        }
    }
}

internal sealed class RelayInfoTable : IAsyncDisposable
{
    private readonly IAnsiConsole _console;
    private LiveDisplayContext? _context;
    private readonly Table _table = new();
    private CancellationTokenSource _cts = new();


    public RelayInfoTable(IAnsiConsole console)
    {
        _console = console;

        _console.Live(_table)
          .AutoClear(false)
          .StartAsync(async ctx =>
          {
              _context = ctx;

              while (!_cts.Token.IsCancellationRequested)
              {
                  ctx.Refresh();
                  await Task.Delay(100, _cts.Token);
              }
          });

        _table.AddColumn("时间");
        _table.AddColumn("输入类型");
        _table.AddColumn("来源");
        _table.AddColumn("内容");
        _table.AddColumn("资源");
        _table.AddColumn("转发");
    }

    public void AddRow()
    {
        var time = new Text(DateTime.Now.ToString("HH:mm:ss"));
        var type = new Text("剪贴板");
        var source = new Text("Pixiv:123456");
        var content = new Text("标题");
        var resource = new Text("资源 [1/2]");
        var replay = new Text("Immich");

        _table.AddRow(time, type, source, content, resource, replay);
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        _cts.Dispose();
    }
}
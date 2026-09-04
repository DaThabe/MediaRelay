using ImageHub.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Spectre.Console;
using System.Text;
using ThabeSoft.Mediator;

namespace ImageHub.Cli;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var host = BuildHost(args);

        await host.StartAsync();

        // 无参数执行, 进入交互式命令行
        if (args.Length == 0)
        {
            await InteractiveAsync(host.Services);
        }
        else
        {
            await TriggeredAsync(host.Services);
        }

        await host.StopAsync();
    }

    // 交互式命令
    private static async Task InteractiveAsync(IServiceProvider service)
    {
        Console.Title = "ImageHub Cli";
        var scopeFactory = service.GetRequiredService<IServiceScopeFactory>();

        while (true)
        {
            // 我是直接从网页拖动过来的, 几乎是瞬间就把网址输入了
            //var url = AnsiConsole.Ask<string>("[bold blue] 请输入网址>>> [/]");

            var url = await ReadInputWithTimeoutAsync(TimeSpan.FromSeconds(0.5));
            if (url == "exit") break;


            await using var scope = scopeFactory.CreateAsyncScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            await sender.SendAsync(new CreateJobCommand(url));
        }
    }


    // 触发式命令
    private static Task TriggeredAsync(IServiceProvider _)
    {
        return Task.CompletedTask;
    }

    // 构建主机
    private static IHost BuildHost(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
#if DEBUG
            .UseEnvironment("development")
#endif
            .ConfigureLogging(logging =>
                logging.ClearProviders())
            .UseSerilog((context, _, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration))
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddImageHub(opts =>
                {
                    var connect_string = configuration.GetConnectionString("Sqlite");
                    var browser_selection = configuration.GetSection("Browser");
                    var resource_selection = configuration.GetSection("Resource");
                    var tg_bot_selection = configuration.GetSection("Telegram");
                    var source_concurrency_selection = configuration.GetSection("SourceConcurrency");

                    opts.Database(x => x.UseSqlite(connect_string));
                    opts.Browser(x => browser_selection.Bind(x));
                    opts.Resource(x => resource_selection.Bind(x));
                    opts.TelegramBot(x => tg_bot_selection.Bind(x));
                    opts.Source(x => source_concurrency_selection.Bind(x));
                });
            })
            .Build();

        return host;
    }


    // 读取输入并设置超时
    private static async Task<string?> ReadInputWithTimeoutAsync(TimeSpan timeout)
    {
        var input = new StringBuilder();
        var lastInputTime = DateTime.Now;
        var hasStarted = false;

        // 监听按键
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.ToString();
                }
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Length--;
                    Console.Write("\b \b");
                    lastInputTime = DateTime.Now; // 退格也重置计时器
                    hasStarted = true;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                    lastInputTime = DateTime.Now; // 有输入重置计时器
                    hasStarted = true;
                }
            }

            // 只有在已经开始输入后，才检查空闲超时
            if (hasStarted && (DateTime.Now - lastInputTime) >= timeout)
            {
                Console.WriteLine();  // 换行
                return input.ToString();  // 自动确认
            }

            await Task.Delay(1);  // 避免CPU空转
        }
    }
}
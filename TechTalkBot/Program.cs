using System.Runtime.InteropServices;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechTalkBot;
using TechTalkBot.Database;
using TechTalkBot.Handlers;
using Telegram.Bot;

var hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Services.AddSingleton<ITelegramBotClient>(svc =>
{
    var token = svc.GetRequiredService<IConfiguration>().GetValue<string>("TelegramToken");
    if (token is null)
    {
        throw new InvalidOperationException("You need to set TelegramToken in configuration");
    }

    return new TelegramBotClient(token);
});
hostBuilder.Services.AddNpgsql<AppDbContext>(hostBuilder.Configuration.GetConnectionString("Npsql"));
hostBuilder.Services.AddHostedService<BotService>()
    .AddMediatR(opt =>
        opt.AddOpenBehavior(typeof(BotCommandPipeline<,>))
            .RegisterServicesFromAssemblyContaining<SuggestHandler>()
    );

var host = hostBuilder.Build();

var cts = new CancellationTokenSource();
PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
{
    if (context.Signal != PosixSignal.SIGINT)
        return;
    context.Cancel = true;
    cts.Cancel();
    cts.Dispose();
});
await host.RunAsync(cts.Token);
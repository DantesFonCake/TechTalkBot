using System.CommandLine.Parsing;
using System.Text;
using CommandLine;
using CommandLine.Text;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechTalkBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Parser = CommandLine.Parser;

namespace TechTalkBot;

public sealed class BotService : BackgroundService
{
    private readonly ITelegramBotClient bot;
    private readonly IMediator mediator;
    private readonly ILogger<BotService> logger;

    public BotService(ITelegramBotClient bot, IMediator mediator, ILogger<BotService> logger)
    {
        this.bot = bot;
        this.mediator = mediator;
        this.logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return bot.ReceiveAsync(
            async (bot, update, token) =>
            {
                try
                {
                    await HandleUpdate(bot, update, token);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Unhandled exception during command handling");
                }
            },
            HandleError,
            new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } },
            stoppingToken);
    }

    private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update is not { Message: { Chat.Id: var chatId, Text: { } text, MessageId: var messageId } })
            return;

        var args = CommandLineStringSplitter.Instance.Split(text);

        var parser = new Parser(conf => conf.HelpWriter = null);
        var parserResult = parser.ParseArguments<Suggest, StartPoll, EndPoll>(args);
        await parserResult
            .WithParsed(obj => LogReceivedCommand(obj, chatId))
            .WithParsedAsync<Suggest>(suggest => mediator.Send(new SuggestRequest
            {
                ChatId = chatId,
                MessageId = messageId,
                Suggest = suggest,
            }, token))
            .WithParsedAsync<StartPoll>(poll => mediator.Send(new StartPollRequest
            {
                ChatId = chatId,
                MessageId = messageId,
                StartPoll = poll,
            }, token))
            .WithParsedAsync<EndPoll>(poll => mediator.Send(new EndPollRequest
            {
                ChatId = chatId,
                MessageId = messageId,
                EndPoll = poll,
            }, token))
            .WithNotParsedAsync(_ => SendHelpText(parserResult, chatId, messageId, token));
    }

    private async Task SendHelpText(ParserResult<object> parserResult, long chatId, int messageId,
        CancellationToken cancellationToken)
    {
        var helpText = HelpText.AutoBuild(parserResult, h =>
        {
            h.AutoHelp = false;
            h.AutoVersion = false;
            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
        }, e => e);

        await bot.SendTextMessageAsync(chatId, helpText.ToString(), replyToMessageId: messageId,
            cancellationToken: cancellationToken);
    }

    private void LogReceivedCommand(object arg, long chatId)
    {
        logger.LogInformation("Received command {CommandType} in chat {ChatId}", arg.GetType(), chatId);
    }

    private async Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        logger.LogError(exception, "Error while polling - will retry later");
        await Task.Delay(TimeSpan.FromSeconds(10), token);
    }
}
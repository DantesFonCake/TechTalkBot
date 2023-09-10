using MediatR;
using TechTalkBot.Commands;
using Telegram.Bot;

namespace TechTalkBot;

public sealed class BotCommandPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBotCommand
{
    private readonly ITelegramBotClient bot;

    public BotCommandPipeline(ITelegramBotClient bot)
    {
        this.bot = bot;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception)
        {
            await bot.SendTextMessageAsync(request.ChatId, "Не получилось выполнить команду, но вы держитесь",
                replyToMessageId: request.MessageId, cancellationToken: cancellationToken);
            throw;
        }
    }
}
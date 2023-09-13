using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechTalkBot.Commands;
using TechTalkBot.Database;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Poll = Telegram.Bot.Types.Poll;

namespace TechTalkBot.Handlers;

public sealed class EndPollHandler : IRequestHandler<EndPollRequest>
{
    private readonly AppDbContext dbContext;
    private readonly ITelegramBotClient bot;
    private readonly ILogger<EndPollHandler> logger;

    public EndPollHandler(AppDbContext dbContext, ITelegramBotClient bot, ILogger<EndPollHandler> logger)
    {
        this.dbContext = dbContext;
        this.bot = bot;
        this.logger = logger;
    }

    public async Task Handle(EndPollRequest request, CancellationToken cancellationToken)
    {
        var chatState = await dbContext.Chats.Include(chat => chat.ActivePoll)
            .ThenInclude(static poll => poll!.Options)
            .FirstOrDefaultAsync(chat => chat.Id == request.ChatId, cancellationToken);
        if (chatState is not { ActivePoll: { } poll })
        {
            await bot.SendTextMessageAsync(request.ChatId,
                "В чате не начато голосование. Перед тем как его закончить надо его начать",
                cancellationToken: cancellationToken);
            return;
        }

        var realPoll = await TryStopPollAsync(request, cancellationToken, poll);
        if (realPoll is null)
        {
            await bot.SendTextMessageAsync(request.ChatId,
                "Что-то пошло не так во время остановки голосования - придётся смотреть победителя самостоятельно",
                replyToMessageId: request.MessageId, cancellationToken: cancellationToken);
            chatState.ActivePoll = null;
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var maxVoters = realPoll.Options.Max(x => x.VoterCount);
        var maxVotedOptions = realPoll.Options.Where(option => option.VoterCount == maxVoters)
            .ToArray();

        var maxVotedOption = maxVotedOptions[Random.Shared.Next(maxVotedOptions.Length)];
        var videoOptionId = ExtractOptionId(maxVotedOption.Text);
        var winnerVideo = poll.Options[videoOptionId];

        await bot.SendTextMessageAsync(request.ChatId, CreateWinnerVideo(winnerVideo), replyToMessageId: poll.Id,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        poll.Winner = winnerVideo;
        poll.EndedAt = DateTimeOffset.UtcNow;
        chatState.ActivePoll = null;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<Poll?> TryStopPollAsync(EndPollRequest request, CancellationToken cancellationToken,
        Database.Poll poll)
    {
        try
        {
            return await bot.StopPollAsync(request.ChatId, poll.Id, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException e) when (e.Message.Contains("poll has already been closed"))
        {
            logger.LogWarning("Poll {PollId} was previously closed", poll.Id);
        }

        return null;
    }

    private string CreateWinnerVideo(Video video) => $"В этот раз смотрим [{video.Name}]({video.Url})";

    private static int ExtractOptionId(string text)
    {
        var idx = text.IndexOf('.');
        if (idx < 0)
            throw new InvalidOperationException("Invalid option format");

        return int.Parse(text.AsSpan()[..idx]) - 1;
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using TechTalkBot.Commands;
using TechTalkBot.Database;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TechTalkBot.Handlers;

public sealed class EndPollHandlers : IRequestHandler<EndPollRequest>
{
    private readonly AppDbContext dbContext;
    private readonly ITelegramBotClient bot;

    public EndPollHandlers(AppDbContext dbContext, ITelegramBotClient bot)
    {
        this.dbContext = dbContext;
        this.bot = bot;
    }

    public async Task Handle(EndPollRequest request, CancellationToken cancellationToken)
    {
        var chatState = await dbContext.Chats.Include(chat => chat.ActivePoll)
            .FirstOrDefaultAsync(chat => chat.Id == request.ChatId, cancellationToken);
        if (chatState is not { ActivePoll: { } poll })
        {
            await bot.SendTextMessageAsync(request.ChatId,
                "В чате не начато голосование. Перед тем как его закончить надо его начать",
                cancellationToken: cancellationToken);
            return;
        }

        var realPoll = await bot.StopPollAsync(request.ChatId, poll.Id, cancellationToken: cancellationToken);
        var maxVoters = realPoll.Options.Max(x => x.VoterCount);
        var maxVotedOptions = realPoll.Options.Where(option => option.VoterCount == maxVoters)
            .ToArray();

        var maxVotedOption = maxVotedOptions[Random.Shared.Next(maxVotedOptions.Length)];
        var videoOptionId = ExtractOptionId(maxVotedOption.Text);
        var winnerVideo = poll.Options[videoOptionId];

        await bot.SendTextMessageAsync(request.ChatId, CreateWinnerVideo(winnerVideo), replyToMessageId: poll.Id, parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        poll.Winner = winnerVideo;
        poll.EndedAt = DateTimeOffset.UtcNow;
        chatState.ActivePoll = null;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TechTalkBot.Commands;
using TechTalkBot.Database;
using Telegram.Bot;

namespace TechTalkBot.Handlers;

public sealed class StartPollHandler : IRequestHandler<StartPollRequest>
{
    private readonly AppDbContext dbContext;
    private readonly ITelegramBotClient bot;

    public StartPollHandler(AppDbContext dbContext, ITelegramBotClient bot)
    {
        this.dbContext = dbContext;
        this.bot = bot;
    }

    private const int MaxPollOptions = 10;

    public async Task Handle(StartPollRequest request, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var chatState = await GetOrCreateChat(request, cancellationToken);
        if (chatState is { ActivePoll: not null })
        {
            await bot.SendTextMessageAsync(request.ChatId,
                "В этом чате уже есть запущенный опрос. Сначала его нужно остановить",
                cancellationToken: cancellationToken);
            return;
        }

        var notWatched = dbContext.Videos.Where(video => !video.Watched);
        var videos = await notWatched.Where(video => !video.WasInPoll)
            .OrderBy(_ => EF.Functions.Random())
            .Take(MaxPollOptions)
            .ToArrayAsync(cancellationToken);
        var remainder = MaxPollOptions - videos.Length;
        if (remainder > 0)
        {
            var oldVideos = await notWatched.Where(video => video.WasInPoll)
                .OrderBy(_ => EF.Functions.Random())
                .Take(remainder)
                .ToArrayAsync(cancellationToken);

            videos = videos.Concat(oldVideos).ToArray();
        }

        if (videos.Length < 2)
        {
            await bot.SendTextMessageAsync(request.ChatId, "Не достаточно видео, чтобы устроить голосование",
                replyToMessageId: request.MessageId, cancellationToken: cancellationToken);
            return;
        }


        var message = await bot.SendPollAsync(request.ChatId, "Что смотрим в следующий раз?",
            videos.Select(CreateOption), replyToMessageId: request.MessageId, cancellationToken: cancellationToken);
        var poll = new Poll
        {
            Id = message.MessageId,
            CreatedAt = DateTimeOffset.UtcNow,
            Options = videos,
        };

        await dbContext.AddAsync(poll, cancellationToken);
        chatState.ActivePoll = poll;
        foreach (var video in videos)
        {
            video.WasInPoll = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<Chat> GetOrCreateChat(StartPollRequest request, CancellationToken cancellationToken)
    {
        var chatState = await dbContext.Chats.Include(chat => chat.ActivePoll)
            .FirstOrDefaultAsync(chat => chat.Id == request.ChatId, cancellationToken);
        if (chatState is null)
        {
            chatState = new Chat
            {
                Id = request.ChatId,
            };

            await dbContext.Chats.AddAsync(chatState, cancellationToken);
        }

        return chatState;
    }

    private static string CreateOption(Video video, int idx)
    {
        var idxPrefix = $"{idx + 1}. ";
        var urlSuffix = $" - {video.Url}";
        var suffixLength = Encoding.UTF8.GetByteCount(urlSuffix);
        if (urlSuffix.Length > 50)
        {
            urlSuffix = "";
            suffixLength = 0;
        }

        var leftSpace = 100 - Encoding.UTF8.GetByteCount(idxPrefix) - suffixLength;
        var name = MakeFitInSpace(video.Name, leftSpace);
        return $"{idxPrefix}{name}{urlSuffix}";
    }

    private static string MakeFitInSpace(string videoName, int leftSpace)
    {
        var videoSpan = videoName.AsSpan();
        while (leftSpace < Encoding.UTF8.GetByteCount(videoSpan))
        {
            videoSpan = videoSpan[..^1];
        }

        return videoSpan.ToString();
    }
}
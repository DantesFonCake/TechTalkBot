using CommandLine;
using MediatR;

namespace TechTalkBot.Commands;

[Verb("/startpoll", HelpText = "Начать голосование за следующий видос")]
public sealed class StartPoll
{
    
}


public sealed class StartPollRequest : IBotCommand
{
    public required StartPoll StartPoll { get; init; }
    public required long ChatId { get; init; }
    public required int MessageId { get; init; }
}
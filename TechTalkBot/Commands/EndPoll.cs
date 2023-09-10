using CommandLine;
using MediatR;

namespace TechTalkBot.Commands;

[Verb("/endpoll", HelpText = "Завершить голосование и вывести результаты")]
public sealed class EndPoll
{
    
}


public sealed class EndPollRequest : IBotCommand
{
    public required EndPoll EndPoll { get; init; }
    public required long ChatId { get; init; }
    public required int MessageId { get; init; }
}
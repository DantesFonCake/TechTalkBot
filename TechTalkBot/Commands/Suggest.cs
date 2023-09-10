using CommandLine;
using MediatR;

namespace TechTalkBot.Commands;

[Verb("/suggest", HelpText = "Предложить видео для просмотра в будущем")]
public sealed class Suggest
{
    [Value(0, Required = true, HelpText = "Название для видео")]
    public required string Name { get; init; }
    
    [Value(1, Required = true, HelpText = "Url для видео")]
    public required Uri Url { get; init; }
}

public sealed class SuggestRequest : IBotCommand
{
    public required Suggest Suggest { get; init; }
    public required long ChatId { get; init; }
    public required int MessageId { get; init; }
}
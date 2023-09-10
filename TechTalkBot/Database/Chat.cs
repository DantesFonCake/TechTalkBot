namespace TechTalkBot.Database;

public sealed class Chat
{
    public required long Id { get; init; }
    public Poll? ActivePoll { get; set; }
}
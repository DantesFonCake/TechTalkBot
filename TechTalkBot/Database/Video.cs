using Microsoft.EntityFrameworkCore;

namespace TechTalkBot.Database;

public sealed class Video
{
    public long Id { get; init; }
    public required string Name { get; init; }
    public required Uri Url { get; init; }
    public bool WasInPoll { get; set; }
    public bool Watched { get; set; }
}
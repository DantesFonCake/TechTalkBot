namespace TechTalkBot.Database;

public sealed class Poll
{
    public required int Id { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required IList<Video> Options { get; init; } = new List<Video>();
    public DateTimeOffset? EndedAt { get; set; }
    public Video? Winner { get; set; }
}
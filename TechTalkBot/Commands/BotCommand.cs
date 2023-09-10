using MediatR;

namespace TechTalkBot.Commands;

public interface IBotCommand : IRequest
{
    public long ChatId { get; }
    public int MessageId { get; }
}
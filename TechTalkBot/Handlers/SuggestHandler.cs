using MediatR;
using TechTalkBot.Commands;
using TechTalkBot.Database;

namespace TechTalkBot.Handlers;

public sealed class SuggestHandler : IRequestHandler<SuggestRequest>
{
    private readonly AppDbContext dbContext;

    public SuggestHandler(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Handle(SuggestRequest request, CancellationToken cancellationToken)
    {
        var video = new Video
        {
            Name = request.Suggest.Name,
            Url = request.Suggest.Url
        };

        await dbContext.Videos.AddAsync(video, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
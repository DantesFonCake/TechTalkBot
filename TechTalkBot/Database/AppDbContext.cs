using Microsoft.EntityFrameworkCore;

namespace TechTalkBot.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<Video> Videos { get; init; } = null!;
    public DbSet<Chat> Chats { get; init; } = null!;
    public DbSet<Poll> Polls { get; init; } = null!;

    public AppDbContext()
    {
        
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Video>()
            .HasKey(video => new { video.Name, video.Url });

        modelBuilder.Entity<Chat>()
            .Property(chat => chat.Id)
            .ValueGeneratedNever();
        modelBuilder.Entity<Poll>()
            .Property(poll => poll.Id)
            .ValueGeneratedNever();
    }
}
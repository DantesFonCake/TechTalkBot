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
        modelBuilder.Entity<Chat>()
            .Property(chat => chat.Id)
            .ValueGeneratedNever();
        modelBuilder.Entity<Poll>()
            .Property(poll => poll.Id)
            .ValueGeneratedNever();
        modelBuilder.Entity<Poll>()
            .HasMany<Video>(poll => poll.Options)
            .WithMany();
    }
}
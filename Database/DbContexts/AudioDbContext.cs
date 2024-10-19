using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using test.Database.Entities;

namespace test.Database.DbContexts;

public class AudioDbContext(DbContextOptions<AudioDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
    }
    public virtual DbSet<Singer> Singers { get; set; }
    public virtual DbSet<Audio> Audios { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Audio>()
        .HasMany(e => e.Singers)
        .WithMany(e => e.Audios);
    }
}

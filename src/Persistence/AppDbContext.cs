using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Model;

namespace Persistence;

public class AppDbContext(string connectionString) : DbContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Race> Race { get; set; }
    public DbSet<Participant> Participant { get; set; }
    public DbSet<Registration> Registration { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Registration>().HasKey(r => new { r.RaceId, r.ParticipantId });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, 
            new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information, DbContextLoggerOptions.None);
        optionsBuilder.UseSqlite(connectionString);
    }
}
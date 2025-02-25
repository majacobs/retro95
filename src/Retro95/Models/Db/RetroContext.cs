using Microsoft.EntityFrameworkCore;

namespace Retro95.Models.Db;

public class RetroContext(DbContextOptions<RetroContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(128);

            entity
                .HasMany(e => e.Teams)
                .WithMany(d => d.Users);
        });
        
        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Team");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name).HasMaxLength(128);
            
            entity.OwnsMany(e => e.DefaultTypes, builder => builder.ToJson());
            
            entity
                .HasMany(e => e.Sessions)
                .WithOne(d => d.Team)
                .HasForeignKey(d => d.TeamId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("Session");

            entity.HasKey(e => e.Id);
        
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            
            entity.OwnsMany(e => e.Types, builder =>
            {
                builder.Property(e => e.Name).HasMaxLength(128);
                builder.ToJson();
            });
            
            entity
                .HasMany(e => e.Comments)
                .WithOne(d => d.Session)
                .HasForeignKey(d => d.SessionId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.HasKey(e => e.Id);
        
            entity.Property(e => e.Text).HasMaxLength(1024);
            entity.Property(e => e.Type).HasMaxLength(128);
            entity.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            
            entity
                .HasOne(e => e.User)
                .WithMany(d => d.Comments)
                .HasForeignKey(d => d.UserId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}
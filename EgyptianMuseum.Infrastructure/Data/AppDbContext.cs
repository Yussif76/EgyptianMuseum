using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ChatConversation> ChatConversations { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<Piece> Pieces { get; set; } = null!;
        public DbSet<ScannedArtifact> ScannedArtifacts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ChatConversation configuration
            modelBuilder.Entity<ChatConversation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Title).HasMaxLength(500);

                entity.HasMany(e => e.Messages)
                    .WithOne(m => m.Conversation)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatMessage configuration
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.SenderType).HasConversion<int>();

                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Piece configuration
            modelBuilder.Entity<Piece>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
                entity.Property(e => e.LabelText).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.ImageUrl).HasMaxLength(1000);
                entity.Property(e => e.Period).HasMaxLength(255);
                entity.Property(e => e.Category).HasMaxLength(255);

                entity.HasIndex(e => e.LabelText).IsUnique();
            });

            // ScannedArtifact configuration
            modelBuilder.Entity<ScannedArtifact>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.LabelText).IsRequired().HasMaxLength(255);

                entity.HasOne(e => e.Piece)
                    .WithMany(p => p.ScannedArtifacts)
                    .HasForeignKey(e => e.PieceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}


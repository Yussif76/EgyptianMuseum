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
        public DbSet<ScannedArtifact> ScannedArtifacts { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<Pieces> pieces { get; set; }
        public DbSet<PieceTranslation> PieceTranslations { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; } = null!;
        public DbSet<Map> Maps { get; set; } = null!;
        public DbSet<IndoorMapPath> IndoorMapPaths { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Pieces>().ToTable("Artifactpieces");

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

            // Pieces configuration
            modelBuilder.Entity<PieceTranslation>()
            .HasOne(x => x.Piece)
            .WithMany(x => x.Translations)
            .HasForeignKey(x => x.PieceId)
            .HasConstraintName("FK_PieceTranslations_Artifactpieces_PieceId");

            modelBuilder.Entity<PieceTranslation>()
                .HasIndex(x => new { x.PieceId, x.LanguageCode })
                .IsUnique();
            modelBuilder.Entity<Pieces>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Pieces>()
            .HasIndex(p => p.Code)
            .IsUnique();

            // ScannedArtifact configuration
            modelBuilder.Entity<ScannedArtifact>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.LabelText).IsRequired().HasMaxLength(255);

                entity.HasOne(e => e.Piece)
                    .WithMany(p => p.ScannedArtifacts)
                    .HasForeignKey(e => e.PieceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ScannedArtifacts_Artifactpieces_PieceId");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.PieceId })
                    .IsUnique()
                    .HasDatabaseName("UK_ScannedArtifacts_UserId_PieceId");
            });

            // Feedback configuration
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.TargetType).HasConversion<int>();
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.Comment).IsRequired().HasMaxLength(1000);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.TargetType, e.TargetId });
            });

            // PasswordResetOtp configuration
            modelBuilder.Entity<PasswordResetOtp>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code).IsRequired().HasMaxLength(6);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ExpiryTime).IsRequired();
                entity.Property(e => e.IsUsed).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.Code, e.IsUsed });
            });

            // Map configuration
            modelBuilder.Entity<Map>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Zone).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ImageUrl).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

                entity.HasMany(e => e.Paths)
                    .WithOne(p => p.Map)
                    .HasForeignKey(p => p.MapId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Zone);
                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            // IndoorMapPath configuration
            modelBuilder.Entity<IndoorMapPath>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MapId).IsRequired();
                entity.Property(e => e.FromRoom).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ToRoom).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FromX).IsRequired();
                entity.Property(e => e.FromY).IsRequired();
                entity.Property(e => e.ToX).IsRequired();
                entity.Property(e => e.ToY).IsRequired();
                entity.Property(e => e.Distance).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

                entity.HasOne(e => e.Map)
                    .WithMany(m => m.Paths)
                    .HasForeignKey(e => e.MapId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.MapId);
                entity.HasQueryFilter(p => !p.IsDeleted);
            });


}
    }
}


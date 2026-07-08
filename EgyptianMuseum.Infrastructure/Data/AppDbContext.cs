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
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<ChatConversation> ChatConversations { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<ScannedArtifact> ScannedArtifacts { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<Pieces> pieces { get; set; }
        public DbSet<PieceTranslation> PieceTranslations { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; } = null!;
        public DbSet<Map> Maps { get; set; } = null!;
        public DbSet<MapTranslation> MapTranslations { get; set; }
        public DbSet<IndoorMapPath> IndoorMapPaths { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<RoomTranslation> RoomTranslations { get; set; }
        public DbSet<Tour> Tours { get; set; } = null!;
        public DbSet<TourTranslation> TourTranslations { get; set; }
        public DbSet<TourPiece> TourPieces { get; set; }
        public DbSet<TourRoom> TourRooms { get; set; } = null!;

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

                entity.HasMany(e => e.Translations)
                    .WithOne(t => t.Map)
                    .HasForeignKey(t => t.MapId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Zone);
                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            // MapTranslation configuration
            modelBuilder.Entity<MapTranslation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MapId).IsRequired();
                entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ZoneName).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Map)
                    .WithMany(m => m.Translations)
                    .HasForeignKey(e => e.MapId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.MapId, e.LanguageCode })
                    .IsUnique();
            });

            // IndoorMapPath configuration
            modelBuilder.Entity<IndoorMapPath>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.MapId).IsRequired();
                entity.Property(e => e.FromRoomId).IsRequired();
                entity.Property(e => e.ToRoomId).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

                entity.HasOne(e => e.Map)
                    .WithMany(m => m.Paths)
                    .HasForeignKey(e => e.MapId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.FromRoom)
                    .WithMany(r => r.FromPaths)
                    .HasForeignKey(e => e.FromRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ToRoom)
                    .WithMany(r => r.ToPaths)
                    .HasForeignKey(e => e.ToRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.MapId);
                entity.HasIndex(e => e.FromRoomId);
                entity.HasIndex(e => e.ToRoomId);
                entity.HasQueryFilter(p => !p.IsDeleted);
            });

            // Room configuration
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.MapId).IsRequired();
                entity.Property(e => e.XCoord).IsRequired();
                entity.Property(e => e.YCoord).IsRequired();
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

                entity.HasOne(e => e.Map)
                    .WithMany(m => m.Rooms)
                    .HasForeignKey(e => e.MapId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Pieces)
                    .WithOne(p => p.Room)
                    .HasForeignKey(p => p.RoomId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.Translations)
                    .WithOne(t => t.Room)
                    .HasForeignKey(t => t.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.MapId);
                entity.HasQueryFilter(r => !r.IsDeleted);
            });

            // RoomTranslation configuration
            modelBuilder.Entity<RoomTranslation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RoomId).IsRequired();
                entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Translations)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.RoomId, e.LanguageCode })
                    .IsUnique();
            });

            // Tour configuration
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.DurationMinutes).IsRequired();
                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ImageUrl).IsRequired();
                entity.Property(e => e.PathImageUrl).IsRequired();
                entity.Property(e => e.MarksJson).IsRequired().HasDefaultValue("[]");
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

                entity.HasMany(e => e.Translations)
                    .WithOne(t => t.Tour)
                    .HasForeignKey(t => t.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.TourPieces)
                    .WithOne(tp => tp.Tour)
                    .HasForeignKey(tp => tp.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.TourRooms)
                    .WithOne(tr => tr.Tour)
                    .HasForeignKey(tr => tr.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Category);
                entity.HasQueryFilter(t => !t.IsDeleted);
            });

            // TourTranslation configuration
            modelBuilder.Entity<TourTranslation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TourId).IsRequired();
                entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Tour)
                    .WithMany(t => t.Translations)
                    .HasForeignKey(e => e.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.TourId, e.LanguageCode })
                    .IsUnique();
            });

            // TourPiece configuration
            modelBuilder.Entity<TourPiece>(entity =>
            {
                entity.HasKey(e => new { e.TourId, e.PieceId });

                entity.Property(e => e.TourId).IsRequired();
                entity.Property(e => e.PieceId).IsRequired();

                entity.HasOne(e => e.Tour)
                    .WithMany(t => t.TourPieces)
                    .HasForeignKey(e => e.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Piece)
                    .WithMany()
                    .HasForeignKey(e => e.PieceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.TourId, e.PieceId })
                    .IsUnique();
            });

            // TourRoom configuration
            modelBuilder.Entity<TourRoom>(entity =>
            {
                entity.HasKey(e => new { e.TourId, e.RoomId });

                entity.Property(e => e.Order).IsRequired();

                entity.HasOne(e => e.Tour)
                    .WithMany(t => t.TourRooms)
                    .HasForeignKey(e => e.TourId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.TourRooms)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.TourId, e.Order });
            });
        }
    }
}


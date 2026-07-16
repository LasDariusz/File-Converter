using Api.Modules.Files.Infrastructure.Persistence.Entities;
using Api.Modules.Users.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Shared.Infrastructure.Persistence;

public class FileConverterContext : DbContext
{
    public FileConverterContext(DbContextOptions<FileConverterContext> options) 
        : base(options) 
    { 
    }

    public DbSet<UserEntity> Users { get; set; }

    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    public DbSet<FileDataEntity> Files { get; set; }

    public DbSet<ConversionJobEntity> Conversions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(user =>
        {
            user.HasIndex(user => user.PublicId)
                .HasDatabaseName("IdxUq_Users_PublicId")
                .IsUnique();

            user.HasIndex(u => u.Email)
                .HasDatabaseName("IdxUq_Users_Email")
                .IsUnique();
        });

        modelBuilder.Entity<RefreshTokenEntity>(token =>
        {
            token.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .HasConstraintName("Fk_UserId_Users")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FileDataEntity>(file =>
        {
            file.HasIndex(f => f.PublicId)
                .HasDatabaseName("IdxUq_Files_PublicId")
                .IsUnique();

            file.HasIndex(f => f.OwnerId)
                .HasDatabaseName("Idx_Files_OwnerId");

            file.HasIndex(f => f.StorageKey)
                .HasDatabaseName("IdxUq_Files_StorageKey")
                .IsUnique();
            
            file.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(f => f.OwnerId)
                .HasConstraintName("Fk_OwnerId_Users")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConversionJobEntity>(cjob => 
        {
            cjob.HasOne<FileDataEntity>()
                .WithMany()
                .HasForeignKey(j => j.SourceFileId)
                .HasConstraintName("Fk_SourceFileId_Files")
                .OnDelete(DeleteBehavior.Restrict);

            cjob.HasOne<FileDataEntity>()
                .WithMany()
                .HasForeignKey(j => j.OutputFileId)
                .HasConstraintName("Fk_OutputFileId_Files")
                .OnDelete(DeleteBehavior.Restrict);

            cjob.Property(j => j.Status)
                .HasConversion<string>()
                .HasMaxLength(32);

            cjob.ToTable(t => t.HasCheckConstraint(
                "Chk_Conversions_Status",
                "\"Status\" IN ('Queued', 'Processing', 'Completed', 'Failed', 'Cancelled')"));
        });
    }

}
using Api.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Database.Context;

public class FileConverterContext : DbContext
{
    public FileConverterContext(DbContextOptions<FileConverterContext> options) 
        : base(options) 
    { 
    }

    public DbSet<UserEntity> Users { get; set; }

    public DbSet<FileDataEntity> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileDataEntity>()
            .HasIndex(f => f.StorageKey)
            .IsUnique();

        modelBuilder.Entity<FileDataEntity>()
            .HasOne(f => f.SourceFile)
            .WithMany(f => f.DerivedFiles)
            .HasForeignKey(f => f.SourceFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}

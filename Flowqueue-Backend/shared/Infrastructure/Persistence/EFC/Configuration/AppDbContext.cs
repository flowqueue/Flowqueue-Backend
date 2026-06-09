using Flowqueue_Backend.Institutions.Domain.Model.Aggregates;
using Flowqueue_Backend.Institutions.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<BranchOffice> BranchOffices => Set<BranchOffice>();
    public DbSet<Service> Services => Set<Service>();

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureInstitutionsContext(builder);
        builder.UseSnakeCaseNamingConvention();
    }

    private static void ConfigureInstitutionsContext(ModelBuilder builder)
    {
        builder.Entity<Institution>(entity =>
        {
            entity.HasKey(institution => institution.Id);
            entity.Property(institution => institution.Id).ValueGeneratedOnAdd();
            entity.Property(institution => institution.Name).IsRequired().HasMaxLength(80);
            entity.Property(institution => institution.Description).IsRequired().HasMaxLength(200);
            entity.Property(institution => institution.Type)
                .HasConversion(type => type.Value, value => new InstitutionType(value))
                .IsRequired()
                .HasMaxLength(40);
            entity.HasIndex(institution => institution.Name).IsUnique();
        });

        builder.Entity<BranchOffice>(entity =>
        {
            entity.HasKey(branchOffice => branchOffice.Id);
            entity.Property(branchOffice => branchOffice.Id).ValueGeneratedOnAdd();
            entity.Property(branchOffice => branchOffice.Name).IsRequired().HasMaxLength(100);
            entity.Property(branchOffice => branchOffice.Address).IsRequired().HasMaxLength(150);
            entity.Property(branchOffice => branchOffice.District).IsRequired().HasMaxLength(80);
            entity.Property(branchOffice => branchOffice.Schedule).IsRequired().HasMaxLength(80);
            entity.HasIndex(branchOffice => new { branchOffice.InstitutionId, branchOffice.Name }).IsUnique();
            entity.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(branchOffice => branchOffice.InstitutionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Service>(entity =>
        {
            entity.HasKey(service => service.Id);
            entity.Property(service => service.Id).ValueGeneratedOnAdd();
            entity.Property(service => service.Name).IsRequired().HasMaxLength(100);
            entity.Property(service => service.AverageDurationMinutes).IsRequired();
            entity.Property(service => service.Prefix)
                .HasConversion(prefix => prefix.Value, value => new ServicePrefix(value))
                .IsRequired()
                .HasMaxLength(5);
            entity.HasIndex(service => new { service.BranchOfficeId, service.Name }).IsUnique();
            entity.HasOne<BranchOffice>()
                .WithMany()
                .HasForeignKey(service => service.BranchOfficeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

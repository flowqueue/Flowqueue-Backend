using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Interceptors;
using  Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Registra el interceptor que pone CreatedAt y UpdatedAt automaticamente
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Aca creas las entidades que vas a manejar 
        builder.UseSnakeCaseNamingConvention();
    }

}   
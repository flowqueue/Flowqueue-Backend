using Flowqueue_Backend.IAM.Application.Internal.CommandServices;
using Flowqueue_Backend.IAM.Domain.Model.Aggregates;
using Flowqueue_Backend.IAM.Domain.Model.Commands;
using Flowqueue_Backend.IAM.Domain.Model.ValueObjects;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Flowqueue_Backend.IAM.Infrastructure.Persistence.EFC.Seeds;

public static class AdminBootstrapper
{
    public static async Task SeedAsync(
        AppDbContext context,
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var enabled = configuration.GetValue("BootstrapAdmin:Enabled", environment.IsDevelopment());
        if (!enabled) return;

        var adminRole = UserRole.NewAdmin();
        var hasAdmin = await context.Users.AnyAsync(
            user => user.Role == adminRole,
            cancellationToken);

        if (hasAdmin) return;

        var email = configuration["BootstrapAdmin:Email"] ?? "admin@flowqueue.pe";
        var password = configuration["BootstrapAdmin:Password"] ?? "Admin123456";
        var fullName = configuration["BootstrapAdmin:FullName"] ?? "FlowQueue Admin";
        var documentNumber = configuration["BootstrapAdmin:DocumentNumber"];

        var command = new CreateUserCommand(
            fullName,
            email,
            password,
            UserRole.NewAdmin(),
            documentNumber);

        context.Users.Add(new User(command, PasswordHashingService.Hash(password)));
        await context.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "Bootstrap admin created for development. Email: {Email}. Change this password after first login.",
            email);
    }
}

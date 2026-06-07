using Flowqueue_Backend.Institutions.Application.Internal.CommandServices;
using Flowqueue_Backend.Institutions.Application.Internal.QueryServices;
using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.Institutions.Infrastructure.Persistence.EFC.Repositories;
using Flowqueue_Backend.shared.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Interfaces.ASP.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddLocalization();
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Connection string not set.");

    var expandedConnectionString = Environment.ExpandEnvironmentVariables(connectionString);

    options.UseMySQL(expandedConnectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IInstitutionRepository, InstitutionRepository>();
builder.Services.AddScoped<IInstitutionCommandService, InstitutionCommandService>();
builder.Services.AddScoped<IInstitutionQueryService, InstitutionQueryService>();

builder.Services.AddScoped<IBranchOfficeRepository, BranchOfficeRepository>();
builder.Services.AddScoped<IBranchOfficeCommandService, BranchOfficeCommandService>();
builder.Services.AddScoped<IBranchOfficeQueryService, BranchOfficeQueryService>();

builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IServiceCommandService, ServiceCommandService>();
builder.Services.AddScoped<IServiceQueryService, ServiceQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

string[] supportedCultures = ["en", "en-US", "es", "es-PE"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

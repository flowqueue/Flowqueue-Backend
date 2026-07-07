using Flowqueue_Backend.Analytics.Application.Internal.QueryServices;
using Flowqueue_Backend.Analytics.Application.Services;
using Flowqueue_Backend.Analytics.Domain.Repositories;
using Flowqueue_Backend.Analytics.Infrastructure.Persistence.EFC.Repositories;
using Flowqueue_Backend.IAM.Application.Internal.CommandServices;
using Flowqueue_Backend.IAM.Application.Internal.QueryServices;
using Flowqueue_Backend.IAM.Application.Services;
using Flowqueue_Backend.IAM.Domain.Repositories;
using Flowqueue_Backend.IAM.Infrastructure.Persistence.EFC.Repositories;
using Flowqueue_Backend.IAM.Infrastructure.Persistence.EFC.Seeds;
using Flowqueue_Backend.IAM.Infrastructure.Tokens;
using Flowqueue_Backend.Institutions.Application.Internal.CommandServices;
using Flowqueue_Backend.Institutions.Application.Internal.QueryServices;
using Flowqueue_Backend.Institutions.Application.Services;
using Flowqueue_Backend.Institutions.Domain.Repositories;
using Flowqueue_Backend.Institutions.Infrastructure.Persistence.EFC.Repositories;
using Flowqueue_Backend.Notifications.Application.Internal.CommandServices;
using Flowqueue_Backend.Notifications.Application.Internal.QueryServices;
using Flowqueue_Backend.Notifications.Application.Services;
using Flowqueue_Backend.Notifications.Domain.Repositories;
using Flowqueue_Backend.Notifications.Infrastructure.Persistence.EFC.Repositories;
using Flowqueue_Backend.Queueing.Application.Internal.CommandServices;
using Flowqueue_Backend.Queueing.Application.Internal.QueryServices;
using Flowqueue_Backend.Queueing.Application.Services;
using Flowqueue_Backend.Queueing.Domain.Repositories;
using Flowqueue_Backend.Queueing.Infrastructure.Persistence.EFC.Repositories;
using Flowqueue_Backend.shared.Domain.Repositories;
using Flowqueue_Backend.shared.Infrastructure.Interfaces.ASP.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Configuration;
using Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configurar el servicio CORS ---
var misReglasCors = "ReglasCorsVercel";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: misReglasCors,
                      policy =>
                      {
                          
                          policy.WithOrigins("https://flowqueue.vercel.app", "http://localhost:5173", "http://127.0.0.1:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
// ---------------------------------------------

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddLocalization();
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresa el token JWT con el formato: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
    });
    options.OperationFilter<AuthorizeOperationFilter>();
});

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("JWT secret must be configured and be at least 32 characters long.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Flowqueue-Backend",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "FlowQueue",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IAuthenticationCommandService, AuthenticationCommandService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddScoped<IInstitutionRepository, InstitutionRepository>();
builder.Services.AddScoped<IInstitutionCommandService, InstitutionCommandService>();
builder.Services.AddScoped<IInstitutionQueryService, InstitutionQueryService>();

builder.Services.AddScoped<IBranchOfficeRepository, BranchOfficeRepository>();
builder.Services.AddScoped<IBranchOfficeCommandService, BranchOfficeCommandService>();
builder.Services.AddScoped<IBranchOfficeQueryService, BranchOfficeQueryService>();

builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IServiceCommandService, ServiceCommandService>();
builder.Services.AddScoped<IServiceQueryService, ServiceQueryService>();

builder.Services.AddScoped<ITurnRepository, TurnRepository>();
builder.Services.AddScoped<ITurnCommandService, TurnCommandService>();
builder.Services.AddScoped<ITurnQueryService, TurnQueryService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IAnalyticsQueryService, AnalyticsQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    await AdminBootstrapper.SeedAsync(
        context,
        app.Configuration,
        app.Environment,
        app.Logger);
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

// --- 2. Aplicar el Middleware de Ruteo y CORS en el orden correcto ---
app.UseRouting();
app.UseCors(misReglasCors);
// ----------------------------------------------------------------------------

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

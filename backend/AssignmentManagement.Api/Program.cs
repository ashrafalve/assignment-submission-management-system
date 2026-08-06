using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using AssignmentManagement.Api.Infrastructure.Authentication;
using AssignmentManagement.Api.Infrastructure.Persistence;
using AssignmentManagement.Api.Infrastructure.Repositories;
using AssignmentManagement.Api.Domain.Interfaces;
using AssignmentManagement.Api.Application.Mapping;
using AssignmentManagement.Api.Application.Interfaces;
using AssignmentManagement.Api.Application.Services;
using AssignmentManagement.Api.Middleware;

// ─── Load .env file (development only) ───────────────────────────────────────
Env.TraversePath().Load();

// ─── Serilog Bootstrap Logger ─────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting AssignmentManagement API...");

    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ─────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    });

    // ─── Configuration ────────────────────────────────────────────────────────
    var configuration = builder.Configuration;
    var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
    builder.Services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

    // ─── Database ─────────────────────────────────────────────────────────────
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null));
    });

    // ─── Repositories & Unit of Work ─────────────────────────────────────────
    builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IClassRepository, ClassRepository>();
    builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
    builder.Services.AddScoped<ITeacherSubjectRepository, TeacherSubjectRepository>();
    builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
    builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();

    // ─── Application Services ─────────────────────────────────────────────────
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IAdminUserService, AdminUserService>();
    builder.Services.AddScoped<IClassService, ClassService>();
    builder.Services.AddScoped<ISubjectService, SubjectService>();
    builder.Services.AddScoped<ITeacherSubjectService, TeacherSubjectService>();
    builder.Services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
    builder.Services.AddScoped<IStudentAssignmentService, StudentAssignmentService>();

    // ─── Infrastructure Services ──────────────────────────────────────────────
    builder.Services.AddScoped<IJwtService, JwtService>();

    // ─── AutoMapper (16+ API) ─────────────────────────────────────────────────────
    builder.Services.AddAutoMapper(cfg =>
    {
        cfg.AddMaps(typeof(MappingProfile).Assembly);
    });

    // ─── FluentValidation ──────────────────────────────────────────────────────
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

    // ─── JWT Authentication ───────────────────────────────────────────────────
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidAudience            = jwtSettings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew                = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // ─── Controllers ──────────────────────────────────────────────────────────
    builder.Services.AddControllers();

    // ─── Swagger / OpenAPI ────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "Assignment Management API",
            Version     = "v1",
            Description = "REST API for managing assignments, users, and submissions.",
            Contact     = new OpenApiContact { Name = "API Support", Email = "support@assignmentmanagement.com" }
        });

        // JWT security definition
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            Type         = SecuritySchemeType.Http,
            Scheme       = "Bearer",
            BearerFormat = "JWT",
            In           = ParameterLocation.Header,
            Description  = "Enter your JWT token. Example: Bearer {token}"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    });

    // ─── Health Checks ────────────────────────────────────────────────────────
    builder.Services.AddHealthChecks()
        .AddNpgSql(
            connectionString: configuration.GetConnectionString("DefaultConnection")!,
            name: "postgresql",
            tags: ["db", "sql", "postgresql"]);

    // ─── CORS ─────────────────────────────────────────────────────────────────
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultCorsPolicy", policy =>
        {
            policy
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()
                             ?? ["http://localhost:3000", "http://localhost:5173"])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // ─── Build App ────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ─── Middleware Pipeline ──────────────────────────────────────────────────
    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Assignment Management API v1");
            options.RoutePrefix = string.Empty; // Swagger at root
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("DefaultCorsPolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Dedicated health check endpoint compatible with HealthChecks UI
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // ─── Auto-migrate on startup (development only) ───────────────────────────
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    Log.Information("AssignmentManagement API started successfully.");
    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}

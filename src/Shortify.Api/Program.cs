using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Shortify.BusinessLogic.DependencyInjection;
using Shortify.DataAccess.DataContext;
using Shortify.DataAccess.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var appConfigConn = builder.Configuration["AppConfigConnectionString"];
    var appConfigEndpoint = builder.Configuration["AppConfigEndpoint"];

    if (!string.IsNullOrEmpty(appConfigConn))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(appConfigConn)
                .Select("ConnectionStrings:*", LabelFilter.Null)
                .Select("ConnectionStrings:*", builder.Environment.EnvironmentName)
        );
    }
    else if (!string.IsNullOrEmpty(appConfigEndpoint))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
                .Select("ConnectionStrings:*", LabelFilter.Null)
                .Select("ConnectionStrings:*", builder.Environment.EnvironmentName)
        );
    }
    else
    {
        throw new InvalidOperationException("Azure App Configuration is not configured: set AppConfigConnectionString or AppConfigEndpoint.");
    }
}

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        var connectionString = builder.Configuration["AppConfigConnectionString"];
        options.Connect(connectionString);
    });
}

var cnt = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.RegisterDataAccessDependencies(cnt!);
builder.Services.RegisterBusinessLogicDependencies();

builder.Services.AddDbContext<ShortifyDbContext>(options =>
    options.UseNpgsql(cnt,
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("Shortify.DataAccess"))
);

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// 5. Read and log the connection string
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
if (string.IsNullOrEmpty(connectionString))
{
    logger.LogCritical("❌ DbConnection is EMPTY! Did you set ConnectionStrings__DbConnection as an environment variable?");
    throw new InvalidOperationException("Connection string 'DbConnection' not found");
}
else
{
    var preview = connectionString.Length > 20
        ? connectionString.Substring(0, 20) + "…"
        : connectionString;
    logger.LogInformation("✅ DbConnection loaded: {connPreview}", preview);
}

// 6. Apply EF Core migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShortifyDbContext>();
    try
    {
        dbContext.Database.Migrate();
        logger.LogInformation("✅ Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Database migration failed.");
        throw;
    }
}

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
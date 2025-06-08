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
    if (!string.IsNullOrEmpty(appConfigConn))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(appConfigConn)
                .Select("ConnectionStrings:*", LabelFilter.Null)
                .Select("ConnectionStrings:*", builder.Environment.EnvironmentName)
        );
    }
    else
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(new Uri("https://shortify-prod.azconfig.io"), new DefaultAzureCredential())
                .Select("ConnectionStrings:*", LabelFilter.Null)
                .Select("ConnectionStrings:*", builder.Environment.EnvironmentName)
        );
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

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.RegisterDataAccessDependencies(connectionString!);
builder.Services.RegisterBusinessLogicDependencies();

builder.Services.AddDbContext<ShortifyDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("Shortify.DataAccess"))
);

var app = builder.Build();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();
using Microsoft.EntityFrameworkCore;
using Shortify.BusinessLogic.DependencyInjection;
using Shortify.DataAccess.DataContext;
using Shortify.DataAccess.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
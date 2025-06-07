using Microsoft.EntityFrameworkCore;
using Shortify.BusinessLogic.DependencyInjection;
using Shortify.DataAccess.DataContext;
using Shortify.DataAccess.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterDataAccessDependencies(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.RegisterBusinessLogicDependencies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ShortifyDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("Shortify.DataAccess"))
);

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
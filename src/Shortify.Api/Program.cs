using Microsoft.EntityFrameworkCore;
using Shortify.DataAccess.DataContext;
using Shortify.DataAccess.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.RegisterDataAccessDependencies(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ShortifyDbContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(options => options.MigrationsAssembly("Shortify.DataAccess")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
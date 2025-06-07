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

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ShortifyDbContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(options => options.MigrationsAssembly("Shortify.DataAccess")));

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using Docosoft.Api.Middlewares;
using Docosoft.Application.Interfaces;
using Docosoft.Application.Services;
using Docosoft.Domain.Repositories;
using Docosoft.Infrastructure.Persistence.Context;
using Docosoft.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Logging;
using System;
using Docosoft.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);

var logsPath = builder.Configuration["LogsPath"];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(logsPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DocosoftStringConnection");

//verifying if db exists and creating db and table if not
DatabaseInitializer.EnsureDatabaseAndTableCreated(connectionString);

builder.Services.AddDbContext<DocosoftDbContext>(options =>
    options.UseSqlServer(connectionString));


//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Services
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IUserService, UserService>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

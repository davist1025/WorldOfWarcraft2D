using Microsoft.EntityFrameworkCore;
using WoW.Database.Models;

var builder = WebApplication.CreateBuilder(args);
var version = ServerVersion.AutoDetect("server=127.0.0.1;uid=root;pwd=1111;database=wpp_auth");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMySql<AuthContext>("server=127.0.0.1;uid=root;pwd=1111;database=wpp_auth", version);

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

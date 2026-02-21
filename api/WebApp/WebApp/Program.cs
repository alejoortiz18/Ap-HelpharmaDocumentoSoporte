using Data;
using Data.DocSoporte;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Data.Common;
using WebApp.DependencyContainer;
using WebApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 👈 importante


builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<DocumentoSoporteData>();
builder.Services.DependencyInjection(); // si la usas

builder.Services.AddSwaggerGen();


builder.Services.DependencyInjection();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // 👈 importante
    app.UseSwaggerUI();    // 👈 importante
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();

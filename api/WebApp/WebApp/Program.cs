using Data;
using Data.DocSoporte;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using WebApp.DependencyContainer;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 👈 importante
builder.Services.AddSwaggerGen();           // 👈 importante

builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<DocumentoSoporteData>();
builder.Services.DependencyInjection(); // si la usas


builder.Services.DependencyInjection();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // 👈 importante
    app.UseSwaggerUI();    // 👈 importante
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // 👈 importante
builder.Services.AddSwaggerGen();           // 👈 importante

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

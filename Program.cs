using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddControllers();

services.AddTransient<ExecutorStorage>();
services.AddScoped<IRuntimeExecutor, PythonExecutor>();
services.AddScoped<IRuntimeExecutor, CExecutor>();

// Собираем приложение
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

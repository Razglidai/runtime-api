using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddControllers();

services.AddTransient<ExecutorStorage>();
services.AddScoped<IRuntimeExecutor, PythonExecutor>();
services.AddScoped<IRuntimeExecutor, CExecutor>();




// Радиоактивная херня
services.AddSwaggerGen();





// Собираем приложение
var app = builder.Build();






// Ща сломается
app.UseSwagger();
app.UseSwaggerUI();







app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Конфигурируем сервисы
Startup.ConfigureServices(builder.Services);

// Собираем приложение
var app = builder.Build();

// Я забыл чё тут должно быть написано
Startup.Configure(app, app.Environment);

app.Run();

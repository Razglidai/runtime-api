var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

// Добавляем сервисы из Startup
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Конфигурируем приложение из Startup
startup.Configure(app, app.Environment);

// Запуск приложения
app.Run();

using System.Reflection;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ---- Регистрация сервисов (DI) ----

// Контроллеры из папки Controllers
builder.Services.AddControllers();

// Описание API для Swagger: адреса берутся из контроллеров,
// пояснения — из комментариев /// в коде (файл backend.xml при сборке)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ИС «Учёт и оценка рисков сервисных учётных записей AD» — API",
        Version = "v1",
        Description = "ЛР7: каркас сервера. Задачи появятся в ЛР8, база данных — в ЛР10.",
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

// Наш сервис: когда контроллеру нужен IAppInfoService, ASP.NET Core подставит AppInfoService.
// Singleton — один объект на всё время работы сервера.
builder.Services.AddSingleton<IAppInfoService, AppInfoService>();

var app = builder.Build();

// ---- Обработка запросов ----

// Swagger только в среде Development: http://localhost:5000/swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Корень http://localhost:5000/ сразу открывает Swagger (в списке адресов его нет)
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

// Адреса контроллеров: GET /health, GET /info
app.MapControllers();

app.Run();

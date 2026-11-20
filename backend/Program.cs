using System.Reflection;
using System.Text.Json.Serialization;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

var builder = WebApplication.CreateBuilder(args);

// ---- Регистрация сервисов (DI) ----

// Контроллеры из папки Controllers
builder.Services
    .AddControllers(options =>
    {
        // В ответе 400 поля называются как в JSON (title), а не как в C# (Title)
        options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
    })
    .AddJsonOptions(options =>
    {
        // Статусы, роли и уровни риска — строками ("New", "High"), а не числами (0, 2)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Ошибки в стандартном формате ProblemDetails (в том числе 500 — без подробностей)
builder.Services.AddProblemDetails();

// Описание API для Swagger: адреса берутся из контроллеров,
// пояснения — из комментариев /// в коде (файл backend.xml при сборке)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ИС «Учёт и оценка рисков сервисных учётных записей AD» — API",
        Version = "v1",
        Description = "ЛР8: задачи и справочник учётных записей. Данные пока в памяти сервера " +
                      "и пропадают после перезапуска; база данных — в ЛР10.",
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

// Наши сервисы: контроллер просит интерфейс, ASP.NET Core подставляет реализацию.
// Singleton — один объект на всё время работы сервера.
builder.Services.AddSingleton<IAppInfoService, AppInfoService>();
// Хранилище в памяти — Singleton, иначе задачи терялись бы после каждого запроса.
// В ЛР10 здесь появится подключение к PostgreSQL.
builder.Services.AddSingleton<InMemoryStore>();
// Правило расчёта риска. Другая модель (например, взвешенная) — замена этой строки.
builder.Services.AddSingleton<IRiskCalculator, RiskCalculator>();
// Scoped — свой объект на каждый запрос (так же будет работать сервис с базой данных)
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IServiceAccountService, ServiceAccountService>();

var app = builder.Build();

// ---- Обработка запросов ----

// Любое необработанное исключение → 500 ProblemDetails без stack trace для клиента
// (подробности остаются только в логе сервера)
app.UseExceptionHandler();

// Swagger только в среде Development: http://localhost:5000/swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Корень http://localhost:5000/ сразу открывает Swagger (в списке адресов его нет)
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

// Адреса контроллеров: /health, /info, /api/tasks…, /api/service-accounts
app.MapControllers();

app.Run();

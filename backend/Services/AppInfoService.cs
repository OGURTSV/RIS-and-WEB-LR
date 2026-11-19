using backend.Models;

namespace backend.Services;

/// <summary>
/// Реализация IAppInfoService. Зарегистрирована в Program.cs как Singleton:
/// один объект на всё время работы сервера, поэтому StartedAt — время запуска.
/// </summary>
public class AppInfoService : IAppInfoService
{
    private readonly IHostEnvironment _environment;
    private readonly DateTimeOffset _startedAt = DateTimeOffset.Now;

    // IHostEnvironment тоже приходит через DI: его регистрирует сам ASP.NET Core
    public AppInfoService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public AppInfo GetInfo() => new(
        Name: "ИС «Учёт и оценка рисков сервисных учётных записей AD»",
        Version: "0.7.0",
        Environment: _environment.EnvironmentName,
        Lab: "ЛР7: каркас сервера; задач и базы данных ещё нет",
        StartedAt: _startedAt);
}

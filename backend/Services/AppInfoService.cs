using backend.Models;

namespace backend.Services;

/// <summary>
/// Реализация IAppInfoService. Зарегистрирована в Program.cs как Singleton:
/// один объект на всё время работы сервера, поэтому StartedAt — время запуска.
/// </summary>
public class AppInfoService : IAppInfoService
{
    private readonly IHostEnvironment _environment;
    // Время старта процесса. Не DateTimeOffset.Now: Singleton создаётся при первом запросе
    // к /info, а не при запуске сервера, и показывал бы время этого запроса.
    private readonly DateTimeOffset _startedAt = System.Diagnostics.Process.GetCurrentProcess().StartTime;

    // IHostEnvironment тоже приходит через DI: его регистрирует сам ASP.NET Core
    public AppInfoService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public AppInfo GetInfo() => new(
        Name: "ИС «Учёт и оценка рисков сервисных учётных записей AD»",
        Version: "0.8.0",
        Environment: _environment.EnvironmentName,
        Lab: "ЛР8: задачи в памяти сервера; базы данных ещё нет",
        StartedAt: _startedAt);
}

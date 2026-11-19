using backend.Models;

namespace backend.Services;

/// <summary>
/// Отдаёт сведения о приложении. Контроллер знает только этот интерфейс,
/// а какая реализация за ним стоит, решает регистрация в Program.cs (DI).
/// </summary>
public interface IAppInfoService
{
    AppInfo GetInfo();
}

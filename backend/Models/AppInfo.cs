namespace backend.Models;

/// <summary>Сведения о приложении — ответ GET /info.</summary>
/// <param name="Name">Название системы.</param>
/// <param name="Version">Версия сервера.</param>
/// <param name="Environment">Среда запуска: Development, Production…</param>
/// <param name="Lab">Учебная пометка: на каком этапе сервер.</param>
/// <param name="StartedAt">Когда запущен процесс сервера.</param>
public record AppInfo(
    string Name,
    string Version,
    string Environment,
    string Lab,
    DateTimeOffset StartedAt);

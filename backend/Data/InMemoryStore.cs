using backend.Models;

namespace backend.Data;

/// <summary>
/// Временное хранилище в памяти сервера (ЛР8). Зарегистрировано в Program.cs как Singleton:
/// один объект на всё время работы, поэтому задачи живут между запросами,
/// но пропадают после перезапуска сервера. В ЛР10 его заменит PostgreSQL.
/// </summary>
public class InMemoryStore
{
    /// <summary>
    /// Замок: запросы обрабатываются параллельно, а List не рассчитан на одновременную запись.
    /// Сервис берёт lock (Sync) на время чтения или изменения задач.
    /// </summary>
    public object Sync { get; } = new();

    /// <summary>«Таблица risk_tasks». При запуске пустая.</summary>
    public List<RiskTask> Tasks { get; } = [];

    /// <summary>Счётчик для id новой задачи — как автоинкремент в базе.</summary>
    public int NextTaskId { get; set; } = 1;

    /// <summary>Справочник учётных записей — тестовые данные (как в docs/api-contract.md, 5.6).</summary>
    public IReadOnlyList<ServiceAccount> ServiceAccounts { get; } =
    [
        new()
        {
            Id = 1, SamAccountName = "svc_sql_1c", OwnerName = "Смирнов П. А.",
            RelatedSystems = "1С, СУБД бухгалтерии",
            IsPrivileged = false, PasswordNeverExpires = false, IsGmsaCompatible = true, IsEnabled = true,
        },
        new()
        {
            Id = 2, SamAccountName = "svc_web", OwnerName = "Кузнецова Е. В.",
            RelatedSystems = "Корпоративный портал",
            IsPrivileged = false, PasswordNeverExpires = true, IsGmsaCompatible = true, IsEnabled = true,
        },
        new()
        {
            Id = 3, SamAccountName = "svc_backup", OwnerName = null,
            RelatedSystems = "Система резервного копирования",
            IsPrivileged = true, PasswordNeverExpires = true, IsGmsaCompatible = true, IsEnabled = true,
        },
    ];

    /// <summary>Пользователи — тестовые данные. Вход по логину и паролю появится позже.</summary>
    public IReadOnlyList<User> Users { get; } =
    [
        new() { Id = 1, Login = "morozov", FullName = "Морозов Д. А.", Role = UserRole.Coordinator },
        new() { Id = 2, Login = "ivanova", FullName = "Иванова А. С.", Role = UserRole.Analyst },
        new() { Id = 3, Login = "sokolova", FullName = "Соколова М. Н.", Role = UserRole.Analyst },
        new() { Id = 4, Login = "volkov", FullName = "Волков С. Е.", Role = UserRole.SysAdmin },
        new() { Id = 5, Login = "petrov", FullName = "Петров И. В.", Role = UserRole.SysAdmin },
    ];
}

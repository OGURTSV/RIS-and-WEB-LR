namespace backend.Models;

/// <summary>Роль пользователя системы (docs/data-model.md, словарь).</summary>
public enum UserRole
{
    /// <summary>Аналитик ИБ: создаёт задачи.</summary>
    Analyst,

    /// <summary>Координатор: назначает исполнителей.</summary>
    Coordinator,

    /// <summary>Системный администратор: выполняет задачи.</summary>
    SysAdmin,
}

/// <summary>Пользователь системы (docs/data-model.md, 2.3). Пароля пока нет — вход появится позже.</summary>
public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string FullName { get; set; } = "";
    public UserRole Role { get; set; }
}

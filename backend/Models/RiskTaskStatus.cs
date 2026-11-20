namespace backend.Models;

/// <summary>
/// Статус задачи. Только эти четыре кода (docs/data-model.md, словарь);
/// русские подписи показывает клиент.
/// </summary>
/// <remarks>
/// Не TaskStatus — такое имя уже занято в .NET (System.Threading.Tasks.TaskStatus).
/// </remarks>
public enum RiskTaskStatus
{
    /// <summary>Зарегистрирована.</summary>
    New,

    /// <summary>В работе.</summary>
    InProgress,

    /// <summary>Риск устранён (конечный статус).</summary>
    Closed,

    /// <summary>Отменена (конечный статус).</summary>
    Cancelled,
}

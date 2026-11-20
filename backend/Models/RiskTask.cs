namespace backend.Models;

/// <summary>
/// Задача по устранению риска — главный объект системы (docs/data-model.md, 2.1).
/// Так задача хранится на сервере. Клиенту отдаётся TaskResponse.
/// </summary>
public class RiskTask
{
    public int Id { get; set; }

    /// <summary>Человеческий номер, например SAR-2026-0001.</summary>
    public string Number { get; set; } = "";

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public RiskTaskStatus Status { get; set; } = RiskTaskStatus.New;

    /// <summary>К какой учётной записи относится риск.</summary>
    public int ServiceAccountId { get; set; }

    /// <summary>Автор задачи (аналитик).</summary>
    public int CreatedByUserId { get; set; }

    /// <summary>Исполнитель; у новой задачи пусто.</summary>
    public int? AssigneeUserId { get; set; }

    /// <summary>Срок, ставится вместе с назначением.</summary>
    public DateOnly? DueDate { get; set; }

    /// <summary>Результат выполнения, обязателен при закрытии.</summary>
    public string? Result { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

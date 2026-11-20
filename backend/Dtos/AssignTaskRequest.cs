using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

/// <summary>Тело PATCH /api/tasks/{id}/assignee — назначение исполнителя. Статус не меняет.</summary>
public class AssignTaskRequest
{
    /// <summary>Id пользователя с ролью SysAdmin.</summary>
    /// <example>5</example>
    [Required(ErrorMessage = "Укажите исполнителя.")]
    [Range(1, int.MaxValue, ErrorMessage = "Идентификатор исполнителя должен быть целым числом больше нуля.")]
    public int? AssigneeUserId { get; set; }

    /// <summary>Срок выполнения (необязательно), не раньше сегодняшнего дня.</summary>
    /// <example>2026-12-01</example>
    public DateOnly? DueDate { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

/// <summary>Тело PATCH /api/tasks/{id}/status — смена статуса. Исполнителя не меняет.</summary>
public class ChangeTaskStatusRequest
{
    /// <summary>Новый статус: New, InProgress, Closed или Cancelled.</summary>
    /// <example>InProgress</example>
    [Required(ErrorMessage = "Укажите статус.")]
    public string? Status { get; set; }

    /// <summary>Результат выполнения — обязателен только при переходе в Closed.</summary>
    /// <example>Учётка переведена на gMSA, флаг PasswordNeverExpires снят.</example>
    [StringLength(1000, ErrorMessage = "Результат — не длиннее 1000 символов.")]
    public string? Result { get; set; }
}

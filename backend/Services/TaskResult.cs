using backend.Dtos;

namespace backend.Services;

/// <summary>Чем закончилась операция с задачей. Контроллер превращает это в HTTP-код.</summary>
public enum TaskOutcome
{
    /// <summary>Успех → 200 / 201.</summary>
    Ok,

    /// <summary>Поле не прошло проверку → 400.</summary>
    Invalid,

    /// <summary>Задачи (или пользователя) с таким id нет → 404.</summary>
    NotFound,

    /// <summary>Запрос корректный, но противоречит правилам процесса → 409.</summary>
    Conflict,
}

/// <summary>
/// Ответ сервиса задач. Сервис ничего не знает про HTTP: он сообщает «не найдено»
/// или «конфликт», а какой код вернуть клиенту, решает контроллер.
/// </summary>
/// <param name="Outcome">Чем закончилось.</param>
/// <param name="Task">Задача — при успехе.</param>
/// <param name="Message">Понятное человеку объяснение отказа.</param>
/// <param name="Field">Для Invalid: какое поле виновато (как в JSON, например "serviceAccountId").</param>
public record TaskResult(TaskOutcome Outcome, TaskResponse? Task = null, string? Message = null, string? Field = null)
{
    public static TaskResult Ok(TaskResponse task) => new(TaskOutcome.Ok, task);
    public static TaskResult Invalid(string field, string message) => new(TaskOutcome.Invalid, Message: message, Field: field);
    public static TaskResult NotFound(string message) => new(TaskOutcome.NotFound, Message: message);
    public static TaskResult Conflict(string message) => new(TaskOutcome.Conflict, Message: message);
}

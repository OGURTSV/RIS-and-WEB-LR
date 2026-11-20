using System.ComponentModel.DataAnnotations;

namespace backend.Dtos;

/// <summary>
/// Тело POST /api/tasks — только поля, которые вводит человек.
/// id, number, status, автора и исполнителя клиент НЕ присылает: их задаёт сервер.
/// </summary>
/// <remarks>
/// Атрибуты [Required], [StringLength], [Range] проверяет сам ASP.NET Core до вызова
/// метода контроллера. Если проверка не прошла — сразу ответ 400 со списком ошибок по полям.
/// </remarks>
public class CreateTaskRequest
{
    /// <summary>Суть риска, 5–80 символов.</summary>
    /// <example>Бессрочный пароль у svc_backup</example>
    [Required(ErrorMessage = "Укажите заголовок.")]
    [StringLength(80, MinimumLength = 5, ErrorMessage = "Заголовок должен содержать от 5 до 80 символов.")]
    public string? Title { get; set; }

    /// <summary>Id сервисной учётной записи из GET /api/service-accounts.</summary>
    /// <example>3</example>
    [Required(ErrorMessage = "Выберите учётную запись.")]
    [Range(1, int.MaxValue, ErrorMessage = "Идентификатор учётной записи должен быть целым числом больше нуля.")]
    public int? ServiceAccountId { get; set; }

    /// <summary>Описание риска и мероприятий, 10–500 символов.</summary>
    /// <example>Пароль не менялся с 2021 года, флаг PasswordNeverExpires. Сменить пароль и перевести учётку на gMSA.</example>
    [Required(ErrorMessage = "Укажите описание.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Описание должно содержать от 10 до 500 символов.")]
    public string? Description { get; set; }
}

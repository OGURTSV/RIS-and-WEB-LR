using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Задачи по устранению рисков — главный объект системы.</summary>
/// <remarks>
/// Контроллер только принимает HTTP и отдаёт ответ; правила процесса — в TaskService.
/// Данные пока в памяти сервера и пропадают после перезапуска (база — ЛР10).
/// </remarks>
[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService tasks, ILogger<TasksController> logger)
    {
        _tasks = tasks;
        _logger = logger;
    }

    /// <summary>Список задач.</summary>
    /// <response code="200">Массив задач; если задач нет — пустой массив [].</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<TaskResponse>> GetAll() => Ok(_tasks.GetAll());

    /// <summary>Одна задача по id.</summary>
    /// <param name="id">Машинный id задачи (не номер SAR-…).</param>
    /// <response code="200">Задача.</response>
    /// <response code="404">Задачи с таким id нет.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TaskResponse> GetById(int id)
    {
        var task = _tasks.GetById(id);
        if (task is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                detail: $"Задача с id = {id} не найдена.");
        return task;
    }

    /// <summary>Создать задачу.</summary>
    /// <remarks>
    /// Клиент присылает только title, serviceAccountId и description.
    /// id, number, status = New и автора задаёт сервер; исполнителя у новой задачи нет.
    /// </remarks>
    /// <response code="201">Задача создана; адрес новой задачи — в заголовке Location.</response>
    /// <response code="400">Поля не прошли проверку или учётной записи с таким id нет.</response>
    /// <response code="500">Неожиданный сбой сервера (без подробностей для клиента).</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<TaskResponse> Create(CreateTaskRequest request)
    {
        try
        {
            var result = _tasks.Create(request);
            if (result.Outcome != TaskOutcome.Ok)
                return ToActionResult(result);

            // 201 Created + заголовок Location: /api/tasks/{id}
            return CreatedAtAction(nameof(GetById), new { id = result.Task!.Id }, result.Task);
        }
        catch (Exception ex)
        {
            // Разработчику — одна понятная строка в логе сервера: операция, id учётки, тип ошибки.
            // Без тела запроса целиком и без stack trace.
            _logger.LogError("CreateTask failed; serviceAccountId={ServiceAccountId}; error={Error}",
                request.ServiceAccountId, ex.GetType().Name);

            // Клиенту — безопасный ответ: ни stack trace, ни путей, ни текста исключения
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: "Внутренняя ошибка сервера",
                detail: "Не удалось создать задачу. Попробуйте позже.");
        }
    }

    /// <summary>Назначить исполнителя. Статус задачи не меняется.</summary>
    /// <param name="id">Id задачи.</param>
    /// <param name="request">Исполнитель (SysAdmin) и необязательный срок.</param>
    /// <response code="200">Исполнитель назначен; задача целиком.</response>
    /// <response code="400">Нет assigneeUserId или срок в прошлом.</response>
    /// <response code="404">Нет задачи или пользователя с таким id.</response>
    /// <response code="409">Задача не в статусе New, пользователь не SysAdmin или он автор задачи.</response>
    [HttpPatch("{id:int}/assignee")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<TaskResponse> Assign(int id, AssignTaskRequest request) =>
        ToActionResult(_tasks.Assign(id, request));

    /// <summary>Сменить статус. Исполнитель не меняется.</summary>
    /// <remarks>
    /// Разрешено: New → InProgress (только если назначен исполнитель), New → Cancelled,
    /// InProgress → Closed (нужен result), InProgress → Cancelled. Остальные переходы — 409.
    /// </remarks>
    /// <param name="id">Id задачи.</param>
    /// <param name="request">Новый статус и, для Closed, результат.</param>
    /// <response code="200">Статус изменён; задача целиком.</response>
    /// <response code="400">Неизвестный статус или Closed без result.</response>
    /// <response code="404">Задачи с таким id нет.</response>
    /// <response code="409">Переход запрещён, например в InProgress без исполнителя.</response>
    [HttpPatch("{id:int}/status")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public ActionResult<TaskResponse> ChangeStatus(int id, ChangeTaskStatusRequest request) =>
        ToActionResult(_tasks.ChangeStatus(id, request));

    // Ответ сервиса → HTTP-код. Ошибки — в стандартном формате ProblemDetails
    private ActionResult<TaskResponse> ToActionResult(TaskResult result)
    {
        switch (result.Outcome)
        {
            case TaskOutcome.Ok:
                return result.Task!;
            case TaskOutcome.Invalid:
                ModelState.AddModelError(result.Field!, result.Message!);
                return ValidationProblem(ModelState);
            case TaskOutcome.NotFound:
                return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found", detail: result.Message);
            default:
                return Problem(statusCode: StatusCodes.Status409Conflict, title: "Conflict", detail: result.Message);
        }
    }
}

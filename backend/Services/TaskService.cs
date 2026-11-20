using backend.Data;
using backend.Dtos;
using backend.Models;

namespace backend.Services;

/// <summary>
/// Сервис задач: хранит правила процесса (docs/api-contract.md, §3 и 5.4–5.5).
/// Контроллер только принимает HTTP и отдаёт ответ, решения принимаются здесь.
/// </summary>
public class TaskService : ITaskService
{
    /// <summary>
    /// Автор новых задач — тестовый аналитик «Иванова А. С.».
    /// Временно: пока нет входа в систему, сервер не знает, кто прислал запрос.
    /// </summary>
    public const int DemoAuthorUserId = 2;

    private readonly InMemoryStore _store;

    public TaskService(InMemoryStore store)
    {
        _store = store;
    }

    public IReadOnlyList<TaskResponse> GetAll()
    {
        lock (_store.Sync)
        {
            // Пустой список — это нормальный ответ 200 и [], а не ошибка
            return _store.Tasks.Select(ToResponse).ToList();
        }
    }

    public TaskResponse? GetById(int id)
    {
        lock (_store.Sync)
        {
            var task = Find(id);
            return task is null ? null : ToResponse(task);
        }
    }

    public TaskResult Create(CreateTaskRequest request)
    {
        // Длины полей и «больше нуля» уже проверил ASP.NET Core по атрибутам CreateTaskRequest.
        // Здесь — то, что атрибутами не проверить: существует ли такая учётка.
        var accountId = request.ServiceAccountId!.Value;
        if (_store.ServiceAccounts.All(a => a.Id != accountId))
            return TaskResult.Invalid("serviceAccountId", $"Учётная запись с id = {accountId} не найдена.");

        lock (_store.Sync)
        {
            var now = DateTimeOffset.UtcNow;
            var id = _store.NextTaskId++;

            var task = new RiskTask
            {
                Id = id,
                Number = $"SAR-{now.Year}-{id:D4}",
                Title = request.Title!.Trim(),
                Description = request.Description!.Trim(),
                ServiceAccountId = accountId,
                // Всё это задаёт сервер, а не клиент:
                Status = RiskTaskStatus.New,
                CreatedByUserId = DemoAuthorUserId,
                AssigneeUserId = null,
                CreatedAt = now,
                UpdatedAt = now,
            };
            _store.Tasks.Add(task);

            return TaskResult.Ok(ToResponse(task));
        }
    }

    public TaskResult Assign(int id, AssignTaskRequest request)
    {
        lock (_store.Sync)
        {
            var task = Find(id);
            if (task is null)
                return TaskResult.NotFound($"Задача с id = {id} не найдена.");

            if (request.DueDate is { } due && due < DateOnly.FromDateTime(DateTime.Today))
                return TaskResult.Invalid("dueDate", "Срок не может быть в прошлом.");

            var userId = request.AssigneeUserId!.Value;
            var user = _store.Users.FirstOrDefault(u => u.Id == userId);
            if (user is null)
                return TaskResult.NotFound($"Пользователь с id = {userId} не найден.");

            // Правила процесса — отказ 409: данные корректные, но так нельзя
            if (task.Status != RiskTaskStatus.New)
                return TaskResult.Conflict($"Назначить исполнителя можно только задаче в статусе New, а у этой — {task.Status}.");
            if (user.Role != UserRole.SysAdmin)
                return TaskResult.Conflict("Исполнителем можно назначить только системного администратора (SysAdmin).");
            if (user.Id == task.CreatedByUserId)
                return TaskResult.Conflict("Автор задачи не может быть назначен её исполнителем.");

            // Меняются только исполнитель и срок — статус остаётся прежним
            task.AssigneeUserId = user.Id;
            task.DueDate = request.DueDate;
            task.UpdatedAt = DateTimeOffset.UtcNow;

            return TaskResult.Ok(ToResponse(task));
        }
    }

    public TaskResult ChangeStatus(int id, ChangeTaskStatusRequest request)
    {
        // Принимаем только четыре кода, написанных ровно так (без "done", "1" и т. п.)
        if (!Enum.GetNames<RiskTaskStatus>().Contains(request.Status))
            return TaskResult.Invalid("status", "Допустимые статусы: New, InProgress, Closed, Cancelled.");
        var newStatus = Enum.Parse<RiskTaskStatus>(request.Status!);

        lock (_store.Sync)
        {
            var task = Find(id);
            if (task is null)
                return TaskResult.NotFound($"Задача с id = {id} не найдена.");

            var from = task.Status;
            var allowed = (from, newStatus) switch
            {
                (RiskTaskStatus.New, RiskTaskStatus.InProgress) => true,
                (RiskTaskStatus.New, RiskTaskStatus.Cancelled) => true,
                (RiskTaskStatus.InProgress, RiskTaskStatus.Closed) => true,
                (RiskTaskStatus.InProgress, RiskTaskStatus.Cancelled) => true,
                _ => false, // всё остальное, в том числе выход из Closed и Cancelled
            };
            if (!allowed)
                return TaskResult.Conflict($"Переход из статуса {from} в {newStatus} запрещён.");

            // Главное правило ЛР8: в работу — только с исполнителем. Это 409, а не 400
            if (newStatus == RiskTaskStatus.InProgress && task.AssigneeUserId is null)
                return TaskResult.Conflict("Нельзя взять задачу в работу, пока не назначен исполнитель.");

            if (newStatus == RiskTaskStatus.Closed)
            {
                if (string.IsNullOrWhiteSpace(request.Result))
                    return TaskResult.Invalid("result", "Чтобы закрыть задачу, опишите результат.");
                task.Result = request.Result.Trim();
            }

            // Меняется только статус — исполнитель остаётся прежним
            task.Status = newStatus;
            task.UpdatedAt = DateTimeOffset.UtcNow;

            return TaskResult.Ok(ToResponse(task));
        }
    }

    private RiskTask? Find(int id) => _store.Tasks.FirstOrDefault(t => t.Id == id);

    private string UserName(int? userId) =>
        _store.Users.FirstOrDefault(u => u.Id == userId)?.FullName ?? $"#{userId}";

    // Как в базе будет JOIN: имена учётки и людей подставляем при чтении, в задаче их нет
    private TaskResponse ToResponse(RiskTask t) => new(
        Id: t.Id,
        Number: t.Number,
        Title: t.Title,
        Description: t.Description,
        Status: t.Status,
        ServiceAccountId: t.ServiceAccountId,
        ServiceAccountName: _store.ServiceAccounts.FirstOrDefault(a => a.Id == t.ServiceAccountId)?.SamAccountName
                            ?? $"#{t.ServiceAccountId}",
        CreatedByUserId: t.CreatedByUserId,
        CreatedByName: UserName(t.CreatedByUserId),
        AssigneeUserId: t.AssigneeUserId,
        AssigneeName: t.AssigneeUserId is null ? null : UserName(t.AssigneeUserId),
        DueDate: t.DueDate,
        Result: t.Result,
        CreatedAt: t.CreatedAt,
        UpdatedAt: t.UpdatedAt);
}

using backend.Dtos;

namespace backend.Services;

/// <summary>Правила процесса для задач: создание, чтение, назначение, смена статуса.</summary>
public interface ITaskService
{
    IReadOnlyList<TaskResponse> GetAll();
    TaskResponse? GetById(int id);
    TaskResult Create(CreateTaskRequest request);
    TaskResult Assign(int id, AssignTaskRequest request);
    TaskResult ChangeStatus(int id, ChangeTaskStatusRequest request);
}

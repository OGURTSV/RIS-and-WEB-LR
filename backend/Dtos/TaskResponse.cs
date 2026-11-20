using backend.Models;

namespace backend.Dtos;

/// <summary>
/// Задача в ответе сервера (docs/api-contract.md, 5.1).
/// Имена учётки, автора и исполнителя сервер подставляет при чтении — в самой задаче их нет.
/// </summary>
public record TaskResponse(
    int Id,
    string Number,
    string Title,
    string Description,
    RiskTaskStatus Status,
    int ServiceAccountId,
    string ServiceAccountName,
    int CreatedByUserId,
    string CreatedByName,
    int? AssigneeUserId,
    string? AssigneeName,
    DateOnly? DueDate,
    string? Result,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

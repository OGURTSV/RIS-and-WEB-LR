using backend.Models;

namespace backend.Dtos;

/// <summary>
/// Учётная запись в ответе GET /api/service-accounts (docs/api-contract.md, 5.6).
/// riskLevel и riskFactors сервер вычисляет при каждом запросе.
/// </summary>
public record ServiceAccountResponse(
    int Id,
    string SamAccountName,
    string? OwnerName,
    string? RelatedSystems,
    bool IsPrivileged,
    bool PasswordNeverExpires,
    bool IsGmsaCompatible,
    bool IsEnabled,
    RiskLevel RiskLevel,
    IReadOnlyList<string> RiskFactors);

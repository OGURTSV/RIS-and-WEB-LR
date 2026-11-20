namespace backend.Models;

/// <summary>Уровень риска учётной записи (docs/data-model.md, 2.5).</summary>
public enum RiskLevel
{
    Low,
    Medium,
    High,
}

/// <summary>
/// Результат оценки риска одной учётки: уровень и коды сработавших факторов.
/// Вычисляется при каждом запросе, нигде не хранится.
/// </summary>
/// <param name="Level">Low, Medium или High.</param>
/// <param name="Factors">Коды факторов: NoOwner, PasswordNeverExpires, Privileged.</param>
public record RiskAssessment(RiskLevel Level, IReadOnlyList<string> Factors);

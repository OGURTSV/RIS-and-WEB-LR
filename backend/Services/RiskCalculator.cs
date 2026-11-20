using backend.Models;

namespace backend.Services;

/// <summary>
/// Текущее правило из docs/data-model.md, 2.5: три фактора с равным весом,
/// уровень — по числу сработавших (0 → Low, 1 → Medium, 2–3 → High).
/// </summary>
public class RiskCalculator : IRiskCalculator
{
    public RiskAssessment Assess(ServiceAccount account)
    {
        var factors = new List<string>();

        if (string.IsNullOrWhiteSpace(account.OwnerName)) factors.Add("NoOwner");
        if (account.PasswordNeverExpires) factors.Add("PasswordNeverExpires");
        if (account.IsPrivileged) factors.Add("Privileged");

        var level = factors.Count switch
        {
            0 => RiskLevel.Low,
            1 => RiskLevel.Medium,
            _ => RiskLevel.High,
        };

        return new RiskAssessment(level, factors);
    }
}

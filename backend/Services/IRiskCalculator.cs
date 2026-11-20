using backend.Models;

namespace backend.Services;

/// <summary>Расчёт уровня риска сервисной учётной записи.</summary>
/// <remarks>
/// Интерфейс нужен, чтобы правило можно было заменить (например, на взвешенную модель
/// R = Σ wᵢ·xᵢ) одной строкой в Program.cs, не трогая контроллеры.
/// </remarks>
public interface IRiskCalculator
{
    RiskAssessment Assess(ServiceAccount account);
}

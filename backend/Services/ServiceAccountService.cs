using backend.Data;
using backend.Dtos;

namespace backend.Services;

/// <summary>
/// Отдаёт справочник учёток. Уровень риска не хранится — считается при каждом запросе,
/// поэтому после изменения признаков учётки следующий GET сразу вернёт новый уровень.
/// </summary>
public class ServiceAccountService : IServiceAccountService
{
    private readonly InMemoryStore _store;
    private readonly IRiskCalculator _risk;

    public ServiceAccountService(InMemoryStore store, IRiskCalculator risk)
    {
        _store = store;
        _risk = risk;
    }

    public IReadOnlyList<ServiceAccountResponse> GetAll() =>
        _store.ServiceAccounts
            .Select(a =>
            {
                var risk = _risk.Assess(a);
                return new ServiceAccountResponse(
                    a.Id, a.SamAccountName, a.OwnerName, a.RelatedSystems,
                    a.IsPrivileged, a.PasswordNeverExpires, a.IsGmsaCompatible, a.IsEnabled,
                    risk.Level, risk.Factors);
            })
            .ToList();
}

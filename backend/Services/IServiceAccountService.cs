using backend.Dtos;

namespace backend.Services;

/// <summary>Справочник сервисных учётных записей с вычисленным уровнем риска.</summary>
public interface IServiceAccountService
{
    IReadOnlyList<ServiceAccountResponse> GetAll();
}

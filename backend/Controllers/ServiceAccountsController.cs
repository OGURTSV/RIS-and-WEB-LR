using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Справочник сервисных учётных записей AD (только чтение).</summary>
[ApiController]
[Route("api/service-accounts")]
[Produces("application/json")]
public class ServiceAccountsController : ControllerBase
{
    private readonly IServiceAccountService _accounts;

    public ServiceAccountsController(IServiceAccountService accounts)
    {
        _accounts = accounts;
    }

    /// <summary>Список учётных записей с уровнем риска.</summary>
    /// <remarks>
    /// riskLevel (Low / Medium / High) и riskFactors сервер вычисляет при каждом запросе
    /// по правилу из docs/data-model.md, 2.5; в данных они не хранятся.
    /// </remarks>
    /// <response code="200">Массив учётных записей; если их нет — [].</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceAccountResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<ServiceAccountResponse>> GetAll() => Ok(_accounts.GetAll());
}

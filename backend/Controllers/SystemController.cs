using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Служебные адреса: жив ли сервер и что это за сервер.</summary>
[ApiController]
public class SystemController : ControllerBase
{
    private readonly IAppInfoService _appInfo;

    // Сервис приходит через конструктор (DI). Контроллер не пишет new AppInfoService():
    // реализацию можно заменить в Program.cs, не трогая этот файл.
    public SystemController(IAppInfoService appInfo)
    {
        _appInfo = appInfo;
    }

    /// <summary>Проверка, что процесс сервера запущен и отвечает (liveness).</summary>
    /// <remarks>
    /// Ответ 200 «ok» означает только, что сервер жив. Он не проверяет создание задач
    /// (POST /api/tasks) и базу данных (ЛР10). Данных задачи этот адрес не принимает.
    /// </remarks>
    /// <response code="200">Сервер работает.</response>
    [HttpGet("/health")]
    [Produces("text/plain")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Health() => Ok("ok");

    /// <summary>Сведения о приложении: название, версия, среда, время запуска.</summary>
    /// <response code="200">Сведения о сервере.</response>
    [HttpGet("/info")]
    [ProducesResponseType(typeof(AppInfo), StatusCodes.Status200OK)]
    public ActionResult<AppInfo> Info() => _appInfo.GetInfo();
}

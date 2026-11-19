# Отчёт по ЛР7. Каркас ASP.NET Core Web API

**Студент:** Огурцов Егор Алексеевич, ТРИС-3-23
**Тема:** ИС «Учёт и оценка рисков сервисных учётных записей AD»

## Что сделано

Проект создан командой `dotnet new webapi -o backend --use-controllers` (.NET SDK 9.0.306, `net9.0`), шаблонный пример `WeatherForecast` удалён.

```text
backend/
├── backend.csproj                  — проект; пакет Swashbuckle.AspNetCore 10.3.1 (Swagger)
├── backend.http                    — готовые запросы GET /health и GET /info
├── Program.cs                      — регистрация сервисов (DI), Swagger, контроллеры
├── appsettings.json, appsettings.Development.json
├── Properties/launchSettings.json  — профиль http: http://localhost:5000, среда Development
├── Controllers/SystemController.cs — GET /health, GET /info
├── Services/IAppInfoService.cs     — интерфейс сервиса
├── Services/AppInfoService.cs      — реализация
├── Models/AppInfo.cs               — модель ответа /info
└── Data/README.md                  — заготовка под хранение (ЛР8 — память, ЛР10 — PostgreSQL)
```

В шаблоне .NET 9 нет Swagger UI, поэтому подключён пакет Swashbuckle.AspNetCore. Версия 11 работает только с .NET 10, для .NET 9 выбрана 10.3.1. Описания адресов в Swagger берутся из комментариев `///` в коде.

`UseHttpsRedirection` из шаблона убран: проверка идёт по HTTP на порту 5000, а без HTTPS-порта эта строка только выводит предупреждение.

## Запуск

```bash
cd backend
dotnet restore
dotnet run
```

Вывод терминала:

```text
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
Hosting environment: Development
```

## Проверки

| № | Проверка | Результат |
|---|---|---|
| 1 | `dotnet build` | 0 ошибок, 0 предупреждений |
| 2 | `dotnet run` | `Now listening on: http://localhost:5000`, среда `Development` |
| 3 | `http://localhost:5000/swagger` | открывается Swagger UI: группа System, `GET /health` и `GET /info` с описаниями, схема `AppInfo` |
| 4 | `http://localhost:5000/` | `302` → `/swagger` |
| 5 | `GET /health` | `200`, `text/plain`, тело `ok` |
| 6 | `GET /info` | `200`, JSON (ниже) |
| 7 | `POST /health` с телом `{"title":"abc"}` | `405 Method Not Allowed` — данные задачи этот адрес не принимает |
| 8 | `git status` | `backend/bin/` и `backend/obj/` не отслеживаются (`.gitignore`) |

Ответ `GET /info`:

```json
{
  "name": "ИС «Учёт и оценка рисков сервисных учётных записей AD»",
  "version": "0.7.0",
  "environment": "Development",
  "lab": "ЛР7: каркас сервера; задач и базы данных ещё нет",
  "startedAt": "2026-11-19T18:41:55.7671268+03:00"
}
```

### Проверка запуска с чистой копии

Проект скачан с GitHub (`git clone`, коммит `4aed63a`) в новую пустую папку, без собранных файлов и локальных настроек. Дальше — только команды из README.

| Что сделано по README | Результат |
|---|---|
| `cd backend`, `dotnet restore` | пакеты восстановлены, ошибок нет |
| `dotnet run` | сборка без ошибок, `Now listening`, `Hosting environment: Development` |
| открыт `/swagger` | `200`, Swagger UI; в описании `/health` и `/info` с текстами из комментариев `///` |
| открыт `/health` | `200`, `text/plain`, `ok` |
| открыт `/info` | `200`, JSON с версией `0.7.0` |

Порт 5000 в момент проверки был занят уже запущенным сервером из рабочей папки, поэтому копия запускалась командой `dotnet run --urls http://localhost:5050`. Сам порт 5000 проверен выше (проверки 2–6).

Вывод: в репозитории есть всё нужное для запуска, ничего не осталось только на компьютере автора.

Проверку другим участником команды (Софья) планируется добавить после того, как она запустит проект у себя.

## Чеклист «/health ≠ создание»

- [x] `GET /health` отвечает `200 ok` — это значит только, что процесс сервера запущен и маршрут отвечает (liveness).
- [x] `/health` не принимает поля задачи (`title`, `serviceAccountId`, `description`): `POST /health` → `405`.
- [x] Успешный `/health` **не** означает, что работает `POST /api/tasks`: такого адреса ещё нет, он появится в ЛР8.
- [x] Успешный `/health` **не** проверяет базу данных: её подключение — ЛР10.
- [x] Создание задачи нигде не выдано за готовое: в Swagger только `/health` и `/info`, в README сказано, что создание появится в ЛР8.

## DI

Сервис `AppInfoService` зарегистрирован через DI, чтобы контроллер не создавал реализацию через `new`.

```csharp
// Program.cs
builder.Services.AddSingleton<IAppInfoService, AppInfoService>();

// Controllers/SystemController.cs
public SystemController(IAppInfoService appInfo)
{
    _appInfo = appInfo;
}
```

Контроллер знает только интерфейс `IAppInfoService`. Когда приходит запрос, ASP.NET Core создаёт контроллер и сам передаёт в конструктор объект `AppInfoService`. Время жизни — `Singleton`: один объект на всё время работы сервера, поэтому `startedAt` показывает время запуска. Сам `AppInfoService` тоже получает через DI встроенный сервис `IHostEnvironment`, из которого берёт название среды.

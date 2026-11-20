# ИС «Учёт и оценка рисков сервисных учётных записей AD»

Курс Б1.В.ДЭ.01.02.02 · КГЭУ · семестр 1 · 2026/2027

## О проекте

Веб-система для инвентаризации сервисных учётных записей Active Directory, оценки связанных с ними рисков информационной безопасности и контроля устранения выявленных нарушений. Система ведёт единый реестр сервисных учётных записей, автоматически рассчитывает уровень риска учётной записи (низкий / средний / высокий) по трём факторам (отсутствие владельца, бессрочный пароль, избыточные привилегии) и формирует задачи для системных администраторов с полным жизненным циклом: создание → назначение → выполнение → закрытие.

## Студент

- **ФИО:** Огурцов Егор Алексеевич
- **Группа:** ТРИС-3-23
- **Тема:** Разработка системы инвентаризации и оценки рисков сервисных учётных записей в среде Active Directory (своя тема)
- **Организация:** Отдел информационной безопасности ООО «Софт решения» (учебный контур)
- **Статус темы:** на проверке (заявка: docs/project-proposal.md)
- **Стек:** Vue 3 + ASP.NET Core + PostgreSQL

## Запуск клиента (Vue)

Нужен Node.js LTS (22.18 или новее); проверить: `node -v`, `npm -v`.

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

`.env` хранит адрес API-сервера (`VITE_API_BASE_URL`) и в Git не попадает; создать его нужно один раз. После изменения `.env` перезапустите `npm run dev`.

Клиент откроется на `http://localhost:5173`. Страницы: `/` — список задач, `/new` — создание задачи, `/login` — вход; каждый адрес можно открыть напрямую. Клиент пока не подключён к серверу: данные отдаёт заглушка `src/api/mockTasks.js` (переключатель `USE_MOCK` в `src/api/tasks.js`), новые задачи видны до перезагрузки страницы (F5). Остановить — `Ctrl+C` в терминале.

После `git pull` снова выполните `npm install`, если в `package.json` появились новые библиотеки (например, `vue-router` в ЛР4).

Vite явно слушает `127.0.0.1` (`frontend/vite.config.js`), чтобы страница открывалась и при включённом системном прокси / VPN.

## Запуск сервера (ASP.NET Core)

Нужен .NET SDK 8 или новее (проект — `net9.0`); проверить: `dotnet --version`.

```bash
cd backend
dotnet restore
dotnet run
```

В терминале должно появиться `Now listening on: http://localhost:5000`. Остановить — `Ctrl+C`.

- **API:** `http://localhost:5000`
- **Swagger:** `http://localhost:5000/swagger` (корень `http://localhost:5000/` переадресует туда же; работает в среде `Development`, она задана в `backend/Properties/launchSettings.json`)
- `GET /health` → `200`, `ok` — проверяет только, что процесс сервера запущен и отвечает (liveness). Создание задач и базу данных он **не** проверяет.
- `GET /info` → название, версия, среда и время запуска сервера.
- Готовые запросы для проверки — в `backend/backend.http` (в PyCharm кнопка ▶ слева от запроса) и коллекция Postman `docs/api-collection/tasks.postman_collection.json` (Import → файл → Run collection).

**Данные пока хранятся в памяти сервера.** После перезапуска (`Ctrl+C`, `dotnet run`) список задач снова пустой, а номера начинаются с `SAR-2026-0001`. Учётные записи и пользователи — тестовые данные, они создаются заново при каждом запуске. База данных PostgreSQL появится в ЛР10.

### Адреса API (ЛР8)

Главный объект — **задача по устранению риска** (`RiskTask`), ресурс `/api/tasks`. Имена полей — из [docs/data-model.md](docs/data-model.md), подробный контракт — [docs/api-contract.md](docs/api-contract.md).

| Метод и путь | Тело | Успех | Отказы |
|---|---|---|---|
| `GET /api/tasks` | — | `200`, массив или `[]` | — |
| `GET /api/tasks/{id}` | — | `200` | `404` |
| `POST /api/tasks` | `title` (5–80), `serviceAccountId` (> 0, есть в справочнике), `description` (10–500) | `201`, `id`, `number`, `status: "New"` | `400` |
| `PATCH /api/tasks/{id}/assignee` | `assigneeUserId`, `dueDate` (необязательно) | `200`, статус не меняется | `400`, `404`, `409` |
| `PATCH /api/tasks/{id}/status` | `status`, `result` (для `Closed`) | `200`, исполнитель не меняется | `400`, `404`, `409` |
| `GET /api/service-accounts` | — | `200`, учётки с `riskLevel` и `riskFactors` | — |

- Статусы: `New`, `InProgress`, `Closed`, `Cancelled`. Переходы: `New` → `InProgress` (только если назначен исполнитель, иначе `409`), `New` → `Cancelled`, `InProgress` → `Closed` (нужен `result`), `InProgress` → `Cancelled`; остальные — `409`.
- Исполнитель — только пользователь с ролью `SysAdmin` (тестовые: `4` Волков С. Е., `5` Петров И. В.) и только у задачи в статусе `New`; иначе `409`.
- `id`, `number`, `status` и автора задаёт сервер: если клиент их пришлёт, они игнорируются. Входа в систему пока нет, поэтому автор новых задач — тестовый аналитик `2` Иванова А. С.
- Удаления (`DELETE`) нет: задача отменяется статусом `Cancelled`.
- Неожиданный сбой → `500` с общим сообщением, без подробностей; причина пишется одной строкой в лог сервера.

## Документы

### ЛР1. Анализ и требования

- [docs/project-proposal.md](docs/project-proposal.md) — заявка на тему
- [docs/lab-01-requirements.md](docs/lab-01-requirements.md) — требования (ТЗ)

### ЛР2. Архитектура и модель данных

- [docs/architecture.md](docs/architecture.md) — схема из трёх узлов: Vue `:5173` → ASP.NET Core `:5000` → PostgreSQL `:5432`
- [docs/data-model.md](docs/data-model.md) — словарь проекта, сущности, ER-диаграмма, проверка 3НФ
- [docs/api-contract.md](docs/api-contract.md) — таблица API, статусы и переходы, примеры JSON
- [docs/requirements-matrix.md](docs/requirements-matrix.md) — критерий ЛР1 → поле → запрос

### ЛР3. Каркас Vue и три экрана

- [frontend/](frontend/) — проект Vue 3 + Vite (JavaScript): экраны «Задачи», «Создать задачу», «Вход»

### ЛР4. Компоненты и маршруты Vue

- [frontend/src/router.js](frontend/src/router.js) — маршруты `/`, `/new`, `/login`
- [frontend/src/App.vue](frontend/src/App.vue) — `AppHeader` снаружи `router-view`
- [frontend/src/components/](frontend/src/components/) — `AppHeader` (меню на `router-link`), `TaskTable` (таблица, данные через prop `items`)
- [frontend/src/pages/](frontend/src/pages/) — страницы `TaskList`, `TaskNew`, `Login`
- [frontend/src/tasksStore.js](frontend/src/tasksStore.js) — единственный общий массив задач

### ЛР5. Форма и список в памяти

- [frontend/src/pages/TaskNew.vue](frontend/src/pages/TaskNew.vue) — форма на `v-model`, проверка полей, ошибки у полей, переход к списку
- `frontend/src/tasksStore.js` — `addItem`: новая задача со статусом `New` и уникальным `id` (в ЛР6 заменено на `createTask` из API-модуля)
- [docs/reports/lab-05.md](docs/reports/lab-05.md) — отчёт: три проверки (успех, отказ, память)

### ЛР6. Клиент API, mock и состояния интерфейса

- [frontend/src/api/tasks.js](frontend/src/api/tasks.js) — `getTasks`, `createTask`, переключатель `USE_MOCK`, разбор ошибок (400 / 500 / нет связи)
- [frontend/src/api/mockTasks.js](frontend/src/api/mockTasks.js) — заглушка сервера с задержкой и учебными отказами
- [frontend/src/pages/TaskList.vue](frontend/src/pages/TaskList.vue) — состояния «загрузка / пусто / данные / ошибка»
- [frontend/src/pages/TaskNew.vue](frontend/src/pages/TaskNew.vue) — блокировка кнопки на время запроса, сообщения об отказах
- [frontend/.env.example](frontend/.env.example) — образец `.env` с адресом API
- [docs/reports/lab-06.md](docs/reports/lab-06.md) — отчёт: проверки состояний и отказов, Network

### ЛР7. Каркас ASP.NET Core Web API

- [backend/](backend/) — проект ASP.NET Core (`net9.0`), порт `5000`, Swagger
- [backend/Program.cs](backend/Program.cs) — регистрация сервисов (DI), Swagger, контроллеры
- [backend/Controllers/SystemController.cs](backend/Controllers/SystemController.cs) — `GET /health`, `GET /info`
- [backend/Services/](backend/Services/) — `IAppInfoService` и его реализация `AppInfoService`
- [backend/Models/AppInfo.cs](backend/Models/AppInfo.cs) — модель ответа `/info`; [backend/Data/](backend/Data/) — заготовка под хранение данных
- [docs/reports/lab-07.md](docs/reports/lab-07.md) — отчёт: проверки, чеклист «/health ≠ создание», DI
### ЛР8. REST: создание, чтение, назначение и статус

- [backend/Controllers/TasksController.cs](backend/Controllers/TasksController.cs) — `/api/tasks`: список, одна задача, создание, два отдельных `PATCH` (`/assignee`, `/status`)
- [backend/Controllers/ServiceAccountsController.cs](backend/Controllers/ServiceAccountsController.cs) — `GET /api/service-accounts` с уровнем риска
- [backend/Dtos/](backend/Dtos/) — что клиент присылает (`CreateTaskRequest`, `AssignTaskRequest`, `ChangeTaskStatusRequest`) и получает (`TaskResponse`, `ServiceAccountResponse`)
- [backend/Services/TaskService.cs](backend/Services/TaskService.cs) — правила процесса (переходы статусов, кого можно назначить)
- [backend/Services/RiskCalculator.cs](backend/Services/RiskCalculator.cs) — расчёт уровня риска учётной записи
- [backend/Data/InMemoryStore.cs](backend/Data/InMemoryStore.cs) — хранилище в памяти и тестовые данные
- [docs/api-collection/tasks.postman_collection.json](docs/api-collection/tasks.postman_collection.json) — коллекция запросов: `201`, `400`, `404`, `409`
- [docs/reports/lab-08.md](docs/reports/lab-08.md) — отчёт: проверки, лог неожиданной ошибки

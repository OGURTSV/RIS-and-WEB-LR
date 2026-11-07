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
npm install
npm run dev
```

Клиент откроется на `http://localhost:5173`. Страницы: `/` — список задач, `/new` — создание задачи, `/login` — вход; каждый адрес можно открыть напрямую. Сервера и базы пока нет: задачи хранятся в памяти браузера, новая задача из формы видна в списке до перезагрузки страницы (F5). Остановить — `Ctrl+C` в терминале.

После `git pull` снова выполните `npm install`, если в `package.json` появились новые библиотеки (например, `vue-router` в ЛР4).

Vite явно слушает `127.0.0.1` (`frontend/vite.config.js`), чтобы страница открывалась и при включённом системном прокси / VPN.

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
- [frontend/src/tasksStore.js](frontend/src/tasksStore.js) — `addItem`: новая задача со статусом `New` и уникальным `id`
- [docs/reports/lab-05.md](docs/reports/lab-05.md) — отчёт: три проверки (успех, отказ, память) и ответы на вопросы
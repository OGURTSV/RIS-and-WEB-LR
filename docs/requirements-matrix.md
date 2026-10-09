# Матрица требований

Курс: Б1.В.ДЭ.01.02.02 · КГЭУ · семестр 1 · 2026/2027  
Студент: Огурцов Егор Алексеевич · Группа: ТРИС-3-23 · ЛР2

Связывает требования из [ЛР1](lab-01-requirements.md) с [моделью данных](data-model.md) и [API](api-contract.md).

---

## 1. Обязательные критерии приёмки

| Требование ЛР1 | Поле или сущность | Запрос | Ожидаемый ответ | Критерий |
|----------------|-------------------|--------|-----------------|----------|
| Аналитик ИБ создаёт задачу по устранению риска | `RiskTask`: `title`, `serviceAccountId` → `ServiceAccount`, `description`; сервер ставит `id`, `number`, `status = New`, `createdByUserId` | `POST /api/tasks` | `201`, `number: "SAR-2026-NNNN"`, `status: "New"`; ошибка — `400` | 1 |
| Координатор назначает системного администратора отдельным действием | `RiskTask.assigneeUserId` → `User` (роль `SysAdmin`), `dueDate` | `PATCH /api/tasks/{id}/assignee` | `200`, исполнитель записан, `status` остался `New`; ошибки — `404`, `409` | 2 |
| Назначенный администратор переводит свою задачу в работу | `RiskTask.status` (`New` → `InProgress`), проверка `assigneeUserId` = текущий пользователь | `PATCH /api/tasks/{id}/status` | `200`, `status: "InProgress"`; чужой логин — `403`, недопустимый переход — `409` | 3 |

## 2. Остальные функциональные требования

| Требование ЛР1 | Поле или сущность | Запрос |
|----------------|-------------------|--------|
| ФТ-1. Создание задачи | `RiskTask`, `ServiceAccount` | `GET /api/service-accounts` (выпадающий список), `POST /api/tasks` |
| ФТ-2. Аналитик видит свои задачи | `RiskTask.createdByUserId` | `GET /api/tasks` (сервер фильтрует по автору) |
| ФТ-3. Аналитик отменяет свою задачу до назначения | `status` (`New` → `Cancelled`), `assigneeUserId = null` | `PATCH /api/tasks/{id}/status` |
| ФТ-4. Координатор видит все задачи, фильтры | `status`, `dueDate` | `GET /api/tasks?status=…`, `GET /api/tasks?overdue=true` |
| ФТ-5. Координатор назначает исполнителя и срок | `assigneeUserId`, `dueDate`, `User.role` | `GET /api/users?role=SysAdmin`, `PATCH /api/tasks/{id}/assignee` |
| ФТ-6. Администратор видит назначенные на него задачи | `RiskTask.assigneeUserId` | `GET /api/tasks` (сервер фильтрует по исполнителю) |
| ФТ-7. «Взять в работу» | `status` (`New` → `InProgress`) | `PATCH /api/tasks/{id}/status` |
| ФТ-8. «Закрыть» с результатом | `status` (`InProgress` → `Closed`), `result` | `PATCH /api/tasks/{id}/status` |
| ФТ-9. Отмена исполнителем (ложное срабатывание) | `status` (`InProgress` → `Cancelled`) | `PATCH /api/tasks/{id}/status` |
| ФТ-10. Запреты по ролям | `User.role`, `createdByUserId`, `assigneeUserId` | любой запрос → `403` |
| ФТ-11. Без входа — только экран входа | `User`, сессия | `POST /api/auth/login`; остальные запросы → `401` |
| ФТ-12. Автор не может быть исполнителем | `createdByUserId` ≠ `assigneeUserId` | `PATCH /api/tasks/{id}/assignee` → `409` |

## 3. Нефункциональные требования, влияющие на модель

| Требование ЛР1 | Что в модели / архитектуре |
|----------------|----------------------------|
| НФТ-1. Данные переживают перезапуск | все сущности хранятся в PostgreSQL |
| НФТ-3. Пароли только хэшем | `users.password_hash`, в ответах API пароля и хэша нет |
| НФТ-4. Ошибка валидации у поля | `400` с объектом `errors` по именам полей (`title`, `description`, …) |
| НФТ-5. Журнал изменений | сущность `TaskHistory`, пишется сервером при `POST` и обоих `PATCH` |

// Заглушка (mock) сервера: отвечает так же, как будущий POST/GET /api/tasks
// (docs/api-contract.md, 5.1–5.2), но без HTTP — прямо в JavaScript.
// Данные живут в памяти вкладки: после F5 они снова начальные.

import { accountName, serviceAccounts } from '../serviceAccounts.js'

// Учебные переключатели. Меняете значение, сохраняете файл — Vite сам обновит страницу.
export const mockSettings = {
  delayMs: 500, // пауза, чтобы успеть увидеть загрузку и заблокированную кнопку
  list: 'ok', // GET:  'ok' | 'empty' | '500' | 'offline'
  create: 'ok', // POST: 'ok' | '400' | '500' | 'offline'
}

// «Таблица risk_tasks» на стороне сервера
const tasks = [
  {
    id: 12,
    number: 'SAR-2026-0012',
    title: 'Учётка svc_backup без владельца',
    description: 'У учётной записи не указан владелец. Найти ответственного и внести в карточку.',
    status: 'InProgress',
    serviceAccountId: 3,
  },
  {
    id: 13,
    number: 'SAR-2026-0013',
    title: 'Бессрочный пароль у svc_web',
    description: 'Снять флаг «Срок действия пароля не ограничен» и сменить пароль.',
    status: 'Closed',
    serviceAccountId: 2,
  },
  {
    id: 14,
    number: 'SAR-2026-0014',
    title: 'Бессрочный пароль у svc_backup',
    description: 'Снять флаг «Срок действия пароля не ограничен» и сменить пароль.',
    status: 'New',
    serviceAccountId: 3,
  },
]
let nextId = 15

const wait = (ms) => new Promise((resolve) => setTimeout(resolve, ms))

// Как сервер отдаёт задачу: имя учётки добавляет JOIN-ом, в таблице его нет
function toResponse(task) {
  return { ...task, serviceAccountName: accountName(task.serviceAccountId) }
}

// Общие учебные отказы. 'offline' — ответа нет совсем: fetch в этом случае
// выбрасывает TypeError, и мы делаем то же самое (это НЕ код 500).
function failure(mode) {
  if (mode === 'offline') throw new TypeError('Failed to fetch')
  if (mode === '500') {
    return { ok: false, status: 500, data: { title: 'Internal Server Error', status: 500 } }
  }
  return null
}

// GET /api/tasks → { ok, status, data: [...] }
export async function getTasks() {
  await wait(mockSettings.delayMs)
  const failed = failure(mockSettings.list)
  if (failed) {
    console.info('[mock без HTTP] GET /api/tasks →', failed.status)
    return failed
  }

  const data = mockSettings.list === 'empty' ? [] : tasks.map(toResponse)
  console.info('[mock без HTTP] GET /api/tasks → 200', data)
  return { ok: true, status: 200, data }
}

// POST /api/tasks → { ok, status: 201, data: задача } или отказ
export async function createTask(body) {
  await wait(mockSettings.delayMs)
  const failed = failure(mockSettings.create)
  if (failed) {
    console.info('[mock без HTTP] POST /api/tasks →', failed.status)
    return failed
  }

  // Сервер проверяет поля сам, даже если клиент уже проверил.
  // create: '400' имитирует отказ сервера при правильных с виду полях.
  const title = String(body.title ?? '').trim()
  const description = String(body.description ?? '').trim()
  const errors = {}
  if (mockSettings.create === '400' || title.length < 5 || title.length > 80) {
    errors.title = ['Заголовок должен содержать от 5 до 80 символов.']
  }
  if (!serviceAccounts.some((account) => account.id === body.serviceAccountId)) {
    errors.serviceAccountId = ['Учётная запись не найдена.']
  }
  if (description.length < 10 || description.length > 500) {
    errors.description = ['Описание должно содержать от 10 до 500 символов.']
  }
  if (Object.keys(errors).length > 0) {
    const data = { title: 'One or more validation errors occurred.', status: 400, errors }
    console.info('[mock без HTTP] POST /api/tasks → 400', data)
    return { ok: false, status: 400, data }
  }

  // id, number и status выдаёт сервер, а не страница
  const id = nextId++
  const task = {
    id,
    number: `SAR-2026-${String(id).padStart(4, '0')}`,
    title,
    description,
    status: 'New',
    serviceAccountId: body.serviceAccountId,
  }
  tasks.push(task)
  console.info('[mock без HTTP] POST /api/tasks → 201', toResponse(task))
  return { ok: true, status: 201, data: toResponse(task) }
}

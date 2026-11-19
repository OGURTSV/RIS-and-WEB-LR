// API-модуль задач. Страницы вызывают только getTasks и createTask
// и не знают, откуда пришли данные: из заглушки или с сервера.

import * as mock from './mockTasks.js'

// Адрес сервера берём из frontend/.env (образец — .env.example).
// После изменения .env нужно перезапустить npm run dev.
const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

// true — ответы даёт заглушка mockTasks.js; false — настоящий fetch к серверу
const USE_MOCK = true

// Сервер ответил, но с ошибкой (400, 500…). Код ответа лежит в status.
export class ApiError extends Error {
  constructor(status, message, fieldErrors = {}) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors // { title: ['…'], … } из ответа 400
  }
}

// Ответа нет совсем: сервер не запущен, неверный адрес, нет сети
export class NetworkError extends Error {
  constructor() {
    super('Нет связи с сервером')
    this.name = 'NetworkError'
  }
}

// Ответ 4xx/5xx — это не исключение fetch: он попадает сюда, и мы проверяем ok сами
function unwrap({ ok, status, data }) {
  if (!ok) throw new ApiError(status, data?.title ?? `HTTP ${status}`, data?.errors)
  return data
}

// Настоящий запрос. fetch выбрасывает исключение, только когда ответа нет вообще.
async function request(path, options = {}) {
  // Content-Type нужен только запросам с телом (POST, PATCH). У GET его нет,
  // иначе браузер перед каждым GET шлёт лишний проверочный запрос OPTIONS.
  const headers = options.body ? { 'Content-Type': 'application/json' } : {}
  let response
  try {
    response = await fetch(`${baseUrl}${path}`, { ...options, headers })
  } catch {
    throw new NetworkError()
  }
  // У ответа может не быть тела (или оно не JSON) — тогда data = null
  const data = await response.json().catch(() => null)
  return unwrap({ ok: response.ok, status: response.status, data })
}

// Вызов заглушки. «Нет связи» она изображает так же, как fetch: выбрасывает TypeError.
async function callMock(mockFunction) {
  let result
  try {
    result = await mockFunction()
  } catch {
    throw new NetworkError()
  }
  return unwrap(result)
}

// Список задач (GET /api/tasks). Возвращает массив, возможно пустой.
export async function getTasks() {
  if (USE_MOCK) return callMock(() => mock.getTasks())
  return request('/api/tasks')
}

// Создание задачи (POST /api/tasks). Возвращает задачу из ответа сервера:
// с id, number и status, которые назначил сервер.
export async function createTask(body) {
  if (USE_MOCK) return callMock(() => mock.createTask(body))
  return request('/api/tasks', { method: 'POST', body: JSON.stringify(body) })
}

// Текст для пользователя: у каждого вида отказа свой
export function errorMessage(error) {
  if (error instanceof NetworkError) {
    return 'Нет связи с сервером. Проверьте подключение и попробуйте ещё раз.'
  }
  if (error instanceof ApiError && error.status === 400) {
    return 'Проверьте поля: сервер не принял данные.'
  }
  if (error instanceof ApiError && error.status >= 500) {
    return 'Сервер не смог обработать запрос. Попробуйте позже.'
  }
  if (error instanceof ApiError) {
    return `Запрос отклонён сервером (код ${error.status}).`
  }
  return 'Непредвиденная ошибка. Подробности — в консоли браузера.'
}

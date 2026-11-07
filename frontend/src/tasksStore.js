import { ref } from 'vue'

// Единственный общий массив задач. Его импортируют страницы, которым нужны задачи;
// таблица получает его только через props.
// Пока это данные в памяти браузера: после F5 модуль создаётся заново,
// и задачи, добавленные через форму, пропадают. Так и должно быть до появления сервера и БД.
// В следующих ЛР вместо них появятся запросы к GET /api/tasks и POST /api/tasks.
export const items = ref([
  {
    id: 12,
    number: 'SAR-2026-0012',
    title: 'Учётка svc_backup без владельца',
    serviceAccountId: 3,
    description: 'У учётной записи не указан владелец. Найти ответственного и внести в карточку.',
    status: 'InProgress',
  },
  {
    id: 13,
    number: 'SAR-2026-0013',
    title: 'Бессрочный пароль у svc_web',
    serviceAccountId: 2,
    description: 'Снять флаг «Срок действия пароля не ограничен» и сменить пароль.',
    status: 'Closed',
  },
  {
    id: 14,
    number: 'SAR-2026-0014',
    title: 'Бессрочный пароль у svc_backup',
    serviceAccountId: 3,
    description: 'Снять флаг «Срок действия пароля не ограничен» и сменить пароль.',
    status: 'New',
  },
])

// Следующий свободный id: на единицу больше самого большого из уже имеющихся,
// поэтому id не повторяются, пока открыта вкладка
let nextId = Math.max(0, ...items.value.map((task) => task.id)) + 1

// Добавляет новую задачу. Номер учебный (TMP-…): настоящий номер SAR-2026-NNNN
// будет выдавать сервер. Новая задача всегда в статусе New («Зарегистрирована»).
export function addItem({ title, serviceAccountId, description }) {
  const id = nextId++
  items.value.push({
    id,
    number: `TMP-${String(id).padStart(4, '0')}`,
    title,
    serviceAccountId,
    description,
    status: 'New',
  })
}

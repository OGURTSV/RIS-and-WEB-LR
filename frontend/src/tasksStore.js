import { ref } from 'vue'

// Единственный общий массив задач. Его импортируют страницы, которым нужны задачи;
// таблица получает его только через props.
// Пока это тестовые данные в памяти: после F5 модуль создаётся заново.
// В следующих ЛР вместо них появятся запросы к GET /api/tasks.
export const items = ref([
  {
    id: 12,
    number: 'SAR-2026-0012',
    title: 'Учётка svc_backup без владельца',
    serviceAccountId: 3,
    status: 'InProgress',
  },
  {
    id: 13,
    number: 'SAR-2026-0013',
    title: 'Бессрочный пароль у svc_web',
    serviceAccountId: 2,
    status: 'Closed',
  },
  {
    id: 14,
    number: 'SAR-2026-0014',
    title: 'Бессрочный пароль у svc_backup',
    serviceAccountId: 3,
    status: 'New',
  },
])

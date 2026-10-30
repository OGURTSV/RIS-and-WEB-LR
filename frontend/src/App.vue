<script setup>
import { ref, computed } from 'vue'

// Подписи кодов на экране — словарь из docs/data-model.md
const statusLabels = {
  New: 'Зарегистрирована',
  InProgress: 'В работе',
  Closed: 'Риск устранён',
  Cancelled: 'Отменена',
}
const riskLabels = { Low: 'Низкий', Medium: 'Средний', High: 'Высокий' }
const riskFactorLabels = {
  NoOwner: 'нет владельца',
  PasswordNeverExpires: 'бессрочный пароль',
  Privileged: 'привилегированная учётная запись',
}

// Тестовые данные вместо GET /api/service-accounts (docs/api-contract.md, 5.6).
// riskLevel и riskFactors потом будет считать сервер, здесь они уже готовые.
const serviceAccounts = [
  { id: 1, samAccountName: 'svc_sql_1c', riskLevel: 'Low', riskFactors: [] },
  { id: 2, samAccountName: 'svc_web', riskLevel: 'Medium', riskFactors: ['PasswordNeverExpires'] },
  {
    id: 3,
    samAccountName: 'svc_backup',
    riskLevel: 'High',
    riskFactors: ['NoOwner', 'PasswordNeverExpires', 'Privileged'],
  },
]

// Тестовые задачи вместо GET /api/tasks
const tasks = [
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
]

// Задача хранит только serviceAccountId, имя учётки берём из справочника (3НФ)
function accountName(id) {
  return serviceAccounts.find((account) => account.id === id)?.samAccountName ?? `#${id}`
}

// Какой экран показан: 'list' | 'new' | 'login'
const screen = ref('list')
const createMessage = ref('')

function show(name) {
  screen.value = name
  createMessage.value = ''
}

// Экран создания: три поля из ЛР2
const form = ref({ title: '', serviceAccountId: null, description: '' })

const selectedAccount = computed(() =>
  serviceAccounts.find((account) => account.id === form.value.serviceAccountId),
)
const selectedFactors = computed(() => {
  const factors = selectedAccount.value?.riskFactors ?? []
  return factors.length ? factors.map((code) => riskFactorLabels[code]).join(', ') : 'нет'
})

function onCreate() {
  // Сохранение появится в ЛР5 (список в памяти) и ЛР6 (POST /api/tasks)
  createMessage.value = 'Форма заполнена. Сохранение подключим в следующих лабораторных.'
}

// Экран входа: настоящая проверка появится вместе с сервером (POST /api/auth/login)
const loginForm = ref({ login: '', password: '' })

// v-focus: ставит курсор в поле, когда экран создания появляется на странице
const vFocus = { mounted: (el) => el.focus() }
</script>

<template>
  <div class="app">
    <header class="app-header">
      <p class="app-title">Риски сервисных учётных записей AD</p>
      <nav aria-label="Экраны">
        <button
          type="button"
          :aria-current="screen === 'list' ? 'page' : undefined"
          @click="show('list')"
        >
          Задачи
        </button>
        <button
          type="button"
          :aria-current="screen === 'new' ? 'page' : undefined"
          @click="show('new')"
        >
          Создать задачу
        </button>
        <button
          type="button"
          :aria-current="screen === 'login' ? 'page' : undefined"
          @click="show('login')"
        >
          Вход
        </button>
      </nav>
    </header>

    <main>
      <!-- Экран 1. Список задач -->
      <section v-if="screen === 'list'" aria-labelledby="list-heading">
        <h1 id="list-heading">Задачи по устранению рисков</h1>
        <div class="table-wrap">
          <table>
            <thead>
              <tr>
                <th scope="col">Номер</th>
                <th scope="col">Заголовок</th>
                <th scope="col">Учётная запись</th>
                <th scope="col">Статус</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="task in tasks" :key="task.id">
                <td>{{ task.number }}</td>
                <td>{{ task.title }}</td>
                <td>{{ accountName(task.serviceAccountId) }}</td>
                <td>
                  <span class="status" :class="`status--${task.status}`">
                    {{ statusLabels[task.status] }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- Экран 2. Создание задачи -->
      <section v-else-if="screen === 'new'" aria-labelledby="new-heading">
        <h1 id="new-heading">Новая задача</h1>
        <form class="form" @submit.prevent="onCreate">
          <label for="title">Заголовок</label>
          <input
            id="title"
            v-model.trim="form.title"
            v-focus
            type="text"
            required
            minlength="5"
            maxlength="80"
            placeholder="Например: Бессрочный пароль у svc_backup"
          />

          <label for="serviceAccountId">Сервисная учётная запись</label>
          <select id="serviceAccountId" v-model.number="form.serviceAccountId" required>
            <option :value="null" disabled>Выберите учётную запись</option>
            <option v-for="account in serviceAccounts" :key="account.id" :value="account.id">
              {{ account.samAccountName }} — {{ riskLabels[account.riskLevel] }}
            </option>
          </select>
          <p v-if="selectedAccount" class="hint">
            Уровень риска: {{ riskLabels[selectedAccount.riskLevel] }}. Факторы:
            {{ selectedFactors }}.
          </p>

          <label for="description">Описание риска и мероприятий</label>
          <textarea
            id="description"
            v-model.trim="form.description"
            required
            minlength="10"
            maxlength="500"
            rows="5"
          ></textarea>

          <button type="submit">Создать</button>
          <p v-if="createMessage" class="hint" role="status">{{ createMessage }}</p>
        </form>
      </section>

      <!-- Экран 3. Вход -->
      <section v-else aria-labelledby="login-heading">
        <h1 id="login-heading">Вход</h1>
        <form class="form" @submit.prevent>
          <label for="login">Логин</label>
          <input
            id="login"
            v-model.trim="loginForm.login"
            type="text"
            autocomplete="username"
            required
          />

          <label for="password">Пароль</label>
          <input
            id="password"
            v-model="loginForm.password"
            type="password"
            autocomplete="current-password"
            required
          />

          <button type="submit">Войти</button>
        </form>
      </section>
    </main>
  </div>
</template>

<style scoped>
.app {
  max-width: 960px;
  margin: 0 auto;
  padding: 16px;
  font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif;
  color: #1f2933;
}

.app-header {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding-bottom: 12px;
  margin-bottom: 20px;
  border-bottom: 1px solid #d9dee5;
}

.app-title {
  margin: 0;
  font-weight: 600;
}

nav {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

button {
  padding: 6px 14px;
  font: inherit;
  color: #4f5bd5;
  background: #fff;
  border: 1px solid #4f5bd5;
  border-radius: 6px;
  cursor: pointer;
}

button[aria-current='page'],
button[type='submit'] {
  color: #fff;
  background: #4f5bd5;
}

h1 {
  margin: 0 0 16px;
  font-size: 1.5rem;
}

.table-wrap {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th,
td {
  padding: 8px 10px;
  text-align: left;
  border-bottom: 1px solid #e4e7eb;
}

th {
  font-weight: 600;
  background: #f5f7fa;
}

.status {
  display: inline-block;
  padding: 2px 8px;
  font-size: 0.9em;
  white-space: nowrap;
  border-radius: 10px;
}

.status--New {
  color: #1d4ed8;
  background: #e6f0ff;
}

.status--InProgress {
  color: #b45309;
  background: #fff4e0;
}

.status--Closed {
  color: #15803d;
  background: #e7f6ec;
}

.status--Cancelled {
  color: #6b7280;
  background: #f1f2f4;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 6px;
  max-width: 520px;
}

.form label {
  margin-top: 10px;
  font-weight: 600;
}

input,
select,
textarea {
  padding: 8px;
  font: inherit;
  border: 1px solid #c5ccd6;
  border-radius: 6px;
}

.form button[type='submit'] {
  align-self: flex-start;
  margin-top: 16px;
}

.hint {
  margin: 4px 0 0;
  font-size: 0.9em;
  color: #52606d;
}
</style>

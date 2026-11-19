<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

import { riskLabels, riskFactorLabels } from '../labels.js'
import { serviceAccounts } from '../serviceAccounts.js'
import { createTask, errorMessage, ApiError } from '../api/tasks.js'

const router = useRouter()

// Три поля из ЛР2; id, number и status выдаёт сервер в ответе
const form = ref({ title: '', serviceAccountId: null, description: '' })
// Текст ошибки для каждого поля; пустая строка — ошибки нет
const errors = ref({ title: '', serviceAccountId: '', description: '' })
// true, пока ждём ответ на POST: кнопка заблокирована, второй клик ничего не делает
const isSubmitting = ref(false)
// Общее сообщение об отказе сервера или отсутствии связи
const submitError = ref('')

const selectedAccount = computed(() =>
  serviceAccounts.find((account) => account.id === form.value.serviceAccountId),
)
const selectedFactors = computed(() => {
  const factors = selectedAccount.value?.riskFactors ?? []
  return factors.length ? factors.map((code) => riskFactorLabels[code]).join(', ') : 'нет'
})

// Проверка полей по правилам из docs/api-contract.md. Возвращает true, если ошибок нет.
function validate(title, serviceAccountId, description) {
  errors.value = { title: '', serviceAccountId: '', description: '' }

  if (title.length < 5 || title.length > 80) {
    errors.value.title = 'Заголовок: от 5 до 80 символов.'
  }
  const accountExists = serviceAccounts.some((account) => account.id === serviceAccountId)
  if (!Number.isInteger(serviceAccountId) || serviceAccountId <= 0 || !accountExists) {
    errors.value.serviceAccountId = 'Выберите учётную запись из списка.'
  }
  if (description.length < 10 || description.length > 500) {
    errors.value.description = 'Описание: от 10 до 500 символов.'
  }

  return !errors.value.title && !errors.value.serviceAccountId && !errors.value.description
}

async function onCreate() {
  // Защита от двойного клика: пока первый запрос не завершился, второй не отправляем
  if (isSubmitting.value) return

  const title = form.value.title.trim()
  const description = form.value.description.trim()
  const serviceAccountId = form.value.serviceAccountId

  // Ранний выход: при ошибке в полях запрос не отправляется, форма остаётся заполненной
  if (!validate(title, serviceAccountId, description)) return

  isSubmitting.value = true
  submitError.value = ''
  try {
    await createTask({ title, serviceAccountId, description })
    // Успех: очищаем форму и переходим к списку — он заново загрузится
    // и покажет задачу с номером, который выдал сервер
    form.value = { title: '', serviceAccountId: null, description: '' }
    router.push('/')
  } catch (error) {
    console.error(error)
    submitError.value = errorMessage(error)
    // При 400 сервер говорит, какое поле не так, — показываем у поля
    if (error instanceof ApiError && error.status === 400) {
      for (const field of Object.keys(errors.value)) {
        errors.value[field] = error.fieldErrors?.[field]?.[0] ?? ''
      }
    }
    // Поля формы не очищаем: пользователь исправит и отправит снова
  } finally {
    // Снимаем блокировку при любом исходе, иначе после ошибки кнопка «зависнет»
    isSubmitting.value = false
  }
}

// v-focus: ставит курсор в первое поле, когда страница открывается
const vFocus = { mounted: (el) => el.focus() }
</script>

<template>
  <main aria-labelledby="new-heading">
    <h1 id="new-heading">Новая задача</h1>
    <!-- novalidate: проверки делает наш код и показывает ошибки у полей,
         а не всплывающие подсказки браузера -->
    <form class="form" novalidate @submit.prevent="onCreate">
      <label for="title">Заголовок</label>
      <input
        id="title"
        v-model="form.title"
        v-focus
        type="text"
        maxlength="80"
        placeholder="Например: Бессрочный пароль у svc_backup"
        :aria-invalid="Boolean(errors.title)"
        aria-describedby="title-error"
        @input="errors.title = ''"
      />
      <p v-if="errors.title" id="title-error" class="error">{{ errors.title }}</p>

      <label for="serviceAccountId">Сервисная учётная запись</label>
      <select
        id="serviceAccountId"
        v-model.number="form.serviceAccountId"
        :aria-invalid="Boolean(errors.serviceAccountId)"
        aria-describedby="account-error"
        @change="errors.serviceAccountId = ''"
      >
        <option :value="null" disabled>Выберите учётную запись</option>
        <option v-for="account in serviceAccounts" :key="account.id" :value="account.id">
          {{ account.samAccountName }} — {{ riskLabels[account.riskLevel] }}
        </option>
      </select>
      <p v-if="errors.serviceAccountId" id="account-error" class="error">
        {{ errors.serviceAccountId }}
      </p>
      <p v-else-if="selectedAccount" class="hint">
        Уровень риска: {{ riskLabels[selectedAccount.riskLevel] }}. Факторы:
        {{ selectedFactors }}.
      </p>

      <label for="description">Описание риска и мероприятий</label>
      <textarea
        id="description"
        v-model="form.description"
        maxlength="500"
        rows="5"
        :aria-invalid="Boolean(errors.description)"
        aria-describedby="description-error"
        @input="errors.description = ''"
      ></textarea>
      <p v-if="errors.description" id="description-error" class="error">
        {{ errors.description }}
      </p>

      <button type="submit" :disabled="isSubmitting">
        {{ isSubmitting ? 'Создаём…' : 'Создать' }}
      </button>
      <p v-if="submitError" class="error" role="alert">{{ submitError }}</p>
    </form>
  </main>
</template>

<style scoped>
.error {
  margin: 4px 0 0;
  font-size: 0.9em;
  color: #b91c1c;
}

[aria-invalid='true'] {
  border-color: #b91c1c;
}

button:disabled {
  cursor: wait;
  opacity: 0.6;
}
</style>

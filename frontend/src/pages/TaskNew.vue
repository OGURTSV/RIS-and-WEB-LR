<script setup>
import { ref, computed } from 'vue'

import { riskLabels, riskFactorLabels } from '../labels.js'
import { serviceAccounts } from '../serviceAccounts.js'

// Три поля из ЛР2; id, number и status клиент не заполняет — их ставит сервер
const form = ref({ title: '', serviceAccountId: null, description: '' })
const createMessage = ref('')

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

// v-focus: ставит курсор в первое поле, когда страница открывается
const vFocus = { mounted: (el) => el.focus() }
</script>

<template>
  <main aria-labelledby="new-heading">
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
  </main>
</template>

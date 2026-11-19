<script setup>
import { ref, onMounted } from 'vue'

import TaskTable from '../components/TaskTable.vue'
import { getTasks, errorMessage } from '../api/tasks.js'
import { items } from '../tasksStore.js'

// Одно поле состояния: 'loading' | 'empty' | 'data' | 'error'.
// В шаблоне ровно одна ветка v-if, поэтому загрузка, таблица и ошибка
// никогда не видны одновременно.
const state = ref('loading')
const loadError = ref('')

async function load() {
  state.value = 'loading'
  try {
    items.value = await getTasks()
    state.value = items.value.length > 0 ? 'data' : 'empty'
  } catch (error) {
    console.error(error)
    loadError.value = errorMessage(error)
    state.value = 'error'
  }
}

// Загружаем список каждый раз при открытии страницы «/»
onMounted(load)
</script>

<template>
  <main aria-labelledby="list-heading">
    <h1 id="list-heading">Задачи по устранению рисков</h1>

    <p v-if="state === 'loading'" class="state" role="status">Загружаем задачи…</p>

    <div v-else-if="state === 'error'" class="state state--error" role="alert">
      <p>Не удалось загрузить задачи. {{ loadError }}</p>
      <button type="button" @click="load">Повторить</button>
    </div>

    <p v-else-if="state === 'empty'" class="state">
      Задач пока нет. <router-link to="/new">Создать первую задачу</router-link>
    </p>

    <!-- state === 'data': массив из tasksStore.js передаём таблице через prop -->
    <TaskTable v-else :items="items" />
  </main>
</template>

<style scoped>
.state {
  padding: 16px;
  color: #52606d;
  background: #f5f7fa;
  border-radius: 6px;
}

.state p {
  margin: 0 0 10px;
}

.state--error {
  color: #b91c1c;
  background: #fdecec;
}
</style>

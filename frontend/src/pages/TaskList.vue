<script setup>
import TaskTable from '../components/TaskTable.vue'
import { items } from '../tasksStore.js'
</script>

<template>
  <main aria-labelledby="list-heading">
    <h1 id="list-heading">Задачи по устранению рисков</h1>
    <!-- Массив живёт в tasksStore.js, страница передаёт его таблице через prop -->
    <TaskTable :items="items" />
  </main>
</template>

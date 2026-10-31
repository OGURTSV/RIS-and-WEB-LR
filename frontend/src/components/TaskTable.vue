<script setup>
import { statusLabels } from '../labels.js'
import { accountName } from '../serviceAccounts.js'

// Таблица своих данных не хранит и массив не меняет: рисует то, что ей передали
defineProps({
  items: { type: Array, required: true },
})
</script>

<template>
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
        <tr v-for="task in items" :key="task.id">
          <td>{{ task.number }}</td>
          <td>{{ task.title }}</td>
          <td>{{ accountName(task.serviceAccountId) }}</td>
          <td>
            <span class="status" :class="`status--${task.status}`">
              {{ statusLabels[task.status] }}
            </span>
          </td>
        </tr>
        <tr v-if="items.length === 0">
          <td colspan="4" class="empty">Задач пока нет</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
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

.empty {
  color: #52606d;
  text-align: center;
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
</style>

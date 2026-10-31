<template>
  <header class="app-header">
    <p class="app-title">Риски сервисных учётных записей AD</p>
    <!-- router-link меняет адрес без перезагрузки страницы; активной ссылке Vue Router сам ставит aria-current="page" -->
    <nav aria-label="Разделы">
      <router-link to="/">Задачи</router-link>
      <router-link to="/new">Создать задачу</router-link>
      <router-link to="/login">Вход</router-link>
    </nav>
  </header>
</template>

<style scoped>
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

a {
  padding: 6px 14px;
  color: #4f5bd5;
  text-decoration: none;
  border: 1px solid #4f5bd5;
  border-radius: 6px;
}

a[aria-current='page'] {
  color: #fff;
  background: #4f5bd5;
}
</style>

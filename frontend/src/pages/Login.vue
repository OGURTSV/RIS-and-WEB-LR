<script setup>
import { ref } from 'vue'

// Настоящая проверка появится вместе с сервером (POST /api/auth/login)
const loginForm = ref({ login: '', password: '' })
</script>

<template>
  <main aria-labelledby="login-heading">
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
  </main>
</template>

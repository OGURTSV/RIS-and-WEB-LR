<script setup>
import AppHeader from './components/AppHeader.vue'
</script>

<template>
  <!-- Шапка снаружи router-view: она одна на все страницы и не перерисовывается при переходах -->
  <AppHeader />
  <router-view />
</template>

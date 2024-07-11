<script setup lang="ts">
import { ref } from 'vue'
import Footer from './components/Footer.vue'
import Header from './components/Header.vue'

import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <Header />

  <div class="container">
    <main role="main" class="pb-3">
      <router-view></router-view>
    </main>
  </div>

  <Footer />
</template>

<style scoped></style>

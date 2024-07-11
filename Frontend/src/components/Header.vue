<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const doLogout = async () => {
  await authStore.logout();
  router.push({ path: '/login' });
};
</script>

<template>
  <header class="sticky-top">
    <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light">
      <div class="container">
        <RouterLink class="navbar-brand" :to="'/'">CapCon</RouterLink>
        <button
          class="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target=".navbar-collapse"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
          <ul class="navbar-nav flex-grow-1">
            <li class="nav-item">
              <RouterLink class="nav-link" :to="'/home'">Home</RouterLink>
            </li>
            <li class="nav-item" v-if="authStore.isAuthenticated">
              <RouterLink class="nav-link" :to="'/companies'">My companies</RouterLink>
            </li>
            <li class="nav-item" v-if="authStore.isAuthenticated">
              <RouterLink class="nav-link" :to="'/loans'">My loans</RouterLink>
            </li>
            <li class="nav-item" v-if="authStore.isAuthenticated">
              <RouterLink class="nav-link" :to="'/loan-requests'">Loan requests</RouterLink>
            </li>
            <li class="nav-item" v-if="authStore.isAuthenticated">
              <RouterLink class="nav-link" :to="'/loan-offers'">My loan offers</RouterLink>
            </li>
          </ul>
          <div class="auth-links">
            <ul v-if="!authStore.isAuthenticated" class="navbar-nav">
              <li class="nav-item">
                <RouterLink class="nav-link" :to="'/register'">Register</RouterLink>
              </li>
              <li class="nav-item">
                <RouterLink class="nav-link" :to="'/login'">Log in</RouterLink>
              </li>
            </ul>
            <ul v-else class="navbar-nav">
              <li class="nav-item">
                <a class="nav-link" href="/Identity/Account/Manage">
                  {{ authStore.userName }}
                </a>
              </li>
              <li class="nav-item">
                <a class="nav-link" style="cursor: pointer" @click="doLogout">
                  Log out
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </nav>
  </header>
</template>
<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()

const loginName = ref('')
const loginPassword = ref('')
const loginOngoing = ref(false)
const errors = ref<string[]>([])

onMounted(() => {
  watch(
    () => authStore.isAuthenticated,
    (isAuthenticated) => {
      if (isAuthenticated) {
        router.push({ path: '/' })
      }
    }
  )
})

const doLogin = async () => {
  errors.value = [] // Clear previous errors
  loginOngoing.value = true

  // Perform the login
  const loginErrors = await authStore.login(loginName.value, loginPassword.value)

  loginOngoing.value = false

  if (loginErrors) {
    errors.value = loginErrors
  } else if (authStore.isAuthenticated) {
    router.push({ path: '/home' })
  }
}
</script>

<template>
  <div class="container">
    <div class="row justify-content-center mt-5">
      <div class="col-md-6">
        <div class="card card-login">
          <div class="card-header">
            <h3 class="card-title mb-0">Log in to CapCon</h3>
          </div>
          <div class="card-body">
            <form @submit.prevent="doLogin">
              <div class="mb-3">
                <label for="inputEmail" class="form-label">Email address</label>
                <input
                  v-model="loginName"
                  type="email"
                  class="form-control"
                  id="inputEmail"
                  placeholder="name@example.com"
                  required
                />
              </div>
              <div class="mb-3">
                <label for="inputPassword" class="form-label">Password</label>
                <input
                  v-model="loginPassword"
                  type="password"
                  class="form-control"
                  id="inputPassword"
                  placeholder="Password"
                  required
                />
              </div>
              <button
                :disabled="loginOngoing"
                type="submit"
                class="btn btn-primary w-100 mb-3"
              >
                {{ loginOngoing ? 'Logging in...' : 'Log in' }}
              </button>
            </form>
            <div class="text-center">
              <p class="mb-0">Don't have an account?</p>
              <RouterLink :to="'/register'" class="btn btn-link">Register here</RouterLink>
            </div>
            <div v-if="errors.length" class="alert alert-danger" role="alert">
              <ul class="mb-0">
                <li v-for="error in errors" :key="error">{{ error }}</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>



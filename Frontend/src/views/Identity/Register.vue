<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth'

const firstName = ref('');
const lastName = ref('');
const email = ref('');
const password = ref('');
const confirmPassword = ref('');
const registrationOngoing = ref(false);
const errors = ref<string[]>([]);

const router = useRouter();
const authStore = useAuthStore()

const validateEmail = (email: string) => {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(email);
};

const doRegister = async () => {
  errors.value = []; 
  
  // Check if all fields are filled
  if (!firstName.value || !lastName.value || !email.value || !password.value || !confirmPassword.value) {
    errors.value.push("All fields are required.");
  }
  
  // Validate email format
  if (!validateEmail(email.value)) {
    errors.value.push("Invalid email format.");
  }
  
  // Check if passwords match
  if (password.value !== confirmPassword.value) {
    errors.value.push("Passwords do not match.");
  }

  // Check password length
  if (password.value.length < 6) {
    errors.value.push("Password must be at least 6 characters long.");
  }

  if (errors.value.length > 0) {
    return;
  }

  registrationOngoing.value = true;

  const registrationErrors = await authStore.register(email.value, password.value, firstName.value, lastName.value);

  registrationOngoing.value = false;

  if (registrationErrors) {
    errors.value = registrationErrors;
  } else if (authStore.isAuthenticated) {
    router.push({ path: '/' });
  }
};
</script>

<template>
  <div class="container">
    <h1></h1>
    <div class="row justify-content-center">
      <div class="col-md-6">
        <form @submit.prevent="doRegister" class="card card-registration">
          <div class="card-header">
            <h3 class="card-title mb-0">Register to CapCon</h3>
          </div>
          <div class="card-body">
            <div class="mb-3" v-if="errors.length">
              <div class="alert alert-danger" role="alert">
                <ul class="mb-0">
                  <li v-for="error in errors" :key="error">{{ error }}</li>
                </ul>
              </div>
            </div>
            <div class="mb-3">
              <input
                v-model="firstName"
                type="text"
                class="form-control"
                placeholder="First name"
                required
              />
            </div>
            <div class="mb-3">
              <input
                v-model="lastName"
                type="text"
                class="form-control"
                placeholder="Last name"
                required
              />
            </div>
            <div class="mb-3">
              <input
                v-model="email"
                type="email"
                class="form-control"
                placeholder="Email"
                required
              />
            </div>
            <div class="mb-3">
              <input
                v-model="password"
                type="password"
                class="form-control"
                placeholder="Password"
                required
              />
            </div>
            <div class="mb-3">
              <input
                v-model="confirmPassword"
                type="password"
                class="form-control"
                placeholder="Confirm Password"
                required
              />
            </div>
            <div class="mb-3">
              <button
                :disabled="registrationOngoing"
                type="submit"
                class="btn btn-primary w-100"
              >
                {{ registrationOngoing ? 'Loading...' : 'Register' }}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>



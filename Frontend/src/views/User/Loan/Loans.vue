<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { BaseService } from '@/services/BaseService'
import { useAuthStore } from '@/stores/auth'
import type { ILoan } from '@/domain/ILoan'
import LoanCard from '@/components/Loan/LoanCard.vue'

const authStore = useAuthStore()
const loans = ref<ILoan[]>()
const errorMessage = ref('')

const loadLoans = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.getAll<ILoan>('loans', jwt!)

    if (response.ok) {
      loans.value = response.data
    } else {
      console.error('Failed to fetch loans:', response.message)
      errorMessage.value = 'Failed to fetch company: ' + response.message
    }
  } catch (error) {
    console.error('Error loading company:', error)
    errorMessage.value = 'Error loading company: ' + error
  }
}

onMounted(async () => {
  await loadLoans()
})
</script>

<template>
  <div class="container">
    <div v-if="errorMessage">
      <h4 class="text-danger">{{ errorMessage }}</h4>
    </div>
    <h2>Your loans</h2>
    <p>
      Here you can view your loans, and add new transactions to them.
      <b
        >NB! When your loan offer gets accepted you have to make the first transaction and wire the
        loan amount to the borrower!</b
      >
    </p>
    <hr />
    <div v-if="loans" class="container">
      <div v-for="loan in loans" v-bind:key="loan.id">
      <LoanCard :company-view="false" :loan="loan"></LoanCard>  
    </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { BaseService } from '@/services/BaseService'
import { useAuthStore } from '@/stores/auth'
import type { ILoanRequest } from '@/domain/ILoanRequest';
import LoanRequestCard from '@/components/LoanRequest/LoanRequestCard.vue'

const authStore = useAuthStore()
const loanRequests = ref<ILoanRequest[]>()

const loadLoanRequests = async () => {
  const response = await BaseService.getAll<ILoanRequest>('loanrequests', authStore.jwt!)
  if (response.ok) {
    loanRequests.value = response.data || []
  } else {
    console.error('Failed to fetch loan requests:', response.message)
  }
}

onMounted(() => {
  loadLoanRequests()
})

</script>

<template>
    <div class="container">
      <h2>Loan requests</h2>
      <p>
        Here you can view active loan offers that companies have published. Come on, help them out and make some money!
      </p>
      <hr>
      <div v-if="loanRequests?.length === 0">There are currenlty no active loan requests, come back later!</div>
      <div class="container">
      <div v-for="request in loanRequests" :key="request.id" class="container" style="padding: 5px;">
        <LoanRequestCard :loan-request="request" :company-view="false" />
      </div>
    </div>
    </div>
  </template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { BaseService } from '@/services/BaseService'
import { useAuthStore } from '@/stores/auth'
import type ICompany from '@/domain/ICompany'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import type { ILoan } from '@/domain/ILoan'
import LoanCard from '@/components/Loan/LoanCard.vue'

const route = useRoute()
const companyId = route.params.id
const authStore = useAuthStore()

const company = ref<ICompany>()
const companyLoans = ref<ILoan[]>()
const userRole = ref<ECompanyRole>(ECompanyRole.Viewer)
const errorMessage = ref('')

const loadCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ICompany>(`companies/${companyId}`, jwt!)
    if (response.ok) {
      company.value = response.data
      companyLoans.value = response.data!.loans
      setUserRole()
    } else {
      console.error('Failed to fetch company:', response.message)
      errorMessage.value = 'Failed to fetch company: ' + response.message
    }
  } catch (error) {
    console.error('Error loading company:', error)
    errorMessage.value = 'Error loading company: ' + error
  }
}

const setUserRole = () => {
  if (!company.value || !authStore.isAuthenticated) {
    return
  }
  const userCompany = company.value.userCompanies.find((uc) => uc.appUserId === authStore.userId)
  if (userCompany) {
    userRole.value = userCompany.role
  }
}

onMounted(async () => {
  await loadCompany()
})
</script>

<template>
  <div class="container">
    <div v-if="errorMessage">
      <h4 class="text-danger">{{ errorMessage }}</h4>
    </div>
    <RouterLink :to="`/company/${company?.id}`" class="btn btn-secondary btn-sm mb-3"
      >Back to company</RouterLink
    >
    <div v-if="company">
      <h3>{{ company.companyName }}'s loans</h3>
    </div>
    <hr />
    <div v-for="loan in companyLoans" v-bind:key="loan.id">
      <LoanCard :loan="loan" :company-view="true"></LoanCard>
    </div>
  </div>
</template>

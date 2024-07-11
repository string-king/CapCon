<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { BaseService } from '@/services/BaseService'
import { useRoute } from 'vue-router'
import type ICompany from '@/domain/ICompany'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import type { ILoanRequest } from '@/domain/ILoanRequest'
import LoanRequestCard from '@/components/LoanRequest/LoanRequestCard.vue'

const route = useRoute()
const companyId = route.params.id
const authStore = useAuthStore()

const company = ref<ICompany>()
const companyActiveRequests = ref<ILoanRequest[]>()
const userRole = ref<ECompanyRole>(ECompanyRole.Viewer)
const errorMessage = ref('')

const loadCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ICompany>(`companies/${companyId}`, jwt!)
    if (response.ok) {
      company.value = response.data
      setActiveRequests()
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

const setActiveRequests = () => {
  if (!company.value) {
    return
  }
  companyActiveRequests.value = company.value.loanRequests.filter((request) => request.active)
}

onMounted(() => {
  loadCompany()
  setUserRole()
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
      <h3>{{ company.companyName }}'s active loan requests</h3>
    </div>
    <hr />
    <div class="container">
      <div v-for="request in companyActiveRequests" :key="request.id" class="container" style="padding: 5px;">
        <LoanRequestCard :loan-request="request" :company-view="true" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { BaseService } from '@/services/BaseService'
import { useRoute } from 'vue-router'
import type ICompany from '@/domain/ICompany'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import type { ILoanRequest } from '@/domain/ILoanRequest'
import LoanOfferCard from '@/components/LoanOffer/LoanOfferCard.vue'

const route = useRoute()
const companyId = route.params.id
const loanRequestId = route.params.loanRequestId
const authStore = useAuthStore()
const userRole = ref<ECompanyRole>(ECompanyRole.Viewer)

const errorMessage = ref('')
const company = ref<ICompany>()
const loanRequest = ref<ILoanRequest>()

const loadCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ICompany>(`companies/${companyId}`, jwt!)
    if (response.ok) {
      company.value = response.data
      setLoanRequest()
    } else {
      console.error('Failed to fetch company:', response.message)
    }
  } catch (error) {
    console.error('Error loading company:', error)
  }
}

const setLoanRequest = () => {
  if (!company.value) {
    console.error('Company is missing')
    return
  }
  loanRequest.value = company.value.loanRequests.find((request) => request.id === loanRequestId)
}

const setUserRole = () => {
  if (!company.value || !authStore.isAuthenticated) {
    return
  }

  const userCompany = company.value.userCompanies.find((uc) => uc.appUserId === authStore.userId)
  if (userCompany) {
    userRole.value = userCompany.role
  } else {
    errorMessage.value = 'You are not authorized to view this page'
  }
}

onMounted(async () => {
  await loadCompany()
  setUserRole()
})
</script>

<template>
  <div v-if="errorMessage">
    <h4 class="text-danger">{{ errorMessage }}</h4>
  </div>
  <RouterLink :to="`/company/${company?.id}/loan-requests`" class="btn btn-secondary btn-sm mb-3"
    >Back to company's requests</RouterLink
  >
  <div class="container">
    <div v-if="company && loanRequest">
      <div class="loan-request-card">
        <div class="loan-request-header">
          <h6>{{ loanRequest.company.companyName }} is asking for:</h6>
        </div>
        <div class="loan-request-details">
          <div class="detail-item">Amount: €{{ loanRequest.amount.toFixed(2) }}</div>
          <div class="detail-item">Interest: {{ loanRequest.interest.toFixed(2) }}%</div>
          <div class="detail-item">
            Created: {{ new Date(loanRequest.createdAt).toLocaleDateString() }}
          </div>
          <div class="detail-item">Period: {{ loanRequest.period }} months</div>
          <div class="detail-item half-width">Comment: {{ loanRequest.comment }}</div>
        </div>
      </div>
    
      <div class="container">
        <h3>Loan Offers</h3>
        <hr />
        <div
          v-for="offer in loanRequest.loanOffers"
          :key="offer.id"
          class="container"
          style="padding: 5px"
        >
          <LoanOfferCard :loanOffer="offer" :companyView="true" :refreshMethod="loadCompany" :role="userRole" :linkToRequest="false" />
        </div>
      </div>
    </div>
  </div>
</template>

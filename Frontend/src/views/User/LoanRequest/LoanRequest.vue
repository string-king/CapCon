<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { BaseService } from '@/services/BaseService'
import { useRoute } from 'vue-router'
import type { ILoanRequest } from '@/domain/ILoanRequest'
import type { ILoanOffer } from '@/domain/ILoanOffer'
import LoanOfferCard from '@/components/LoanOffer/LoanOfferCard.vue'

const route = useRoute()
const loanRequestId = route.params.id
const authStore = useAuthStore()
const errorMessage = ref('')
const loanRequest = ref<ILoanRequest>()
const showForm = ref(false) // State for toggling form visibility

const newOffer = ref<ILoanOffer>({
  loanRequestId: loanRequestId.toString(),
  appUserId: authStore.getUserId,
  amount: 0,
  interest: 0,
  period: 0,
  comment: '',
  active: true,
  createdAt: new Date()
})

const loadLoanRequest = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ILoanRequest>(`loanrequests/${loanRequestId}`, jwt!)
    if (response.ok) {
      loanRequest.value = response.data
      setNewOfferData()
    } else {
      errorMessage.value = 'Failed to fetch loan request'
    }
  } catch (error) {
    errorMessage.value = 'Error loading loan request'
  }
}

const setNewOfferData = () => {
  if (!loanRequest.value) {
    errorMessage.value = 'Loan request is missing'
    return
  }
  if (!authStore.isAuthenticated) {
    errorMessage.value = 'User is not authenticated'
    return
  }
  newOffer.value = {
    loanRequestId: loanRequestId.toString(),
    appUserId: authStore.getUserId,
    amount: loanRequest.value.amount,
    interest: loanRequest.value.interest,
    period: loanRequest.value.period,
    comment: '',
    active: true,
    createdAt: new Date()
  }
}

const createLoanOffer = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.create<ILoanOffer>(newOffer.value, '/loanoffers', jwt!)
    if (response.ok) {
      loadLoanRequest()
      showForm.value = false
    } else {
      errorMessage.value = response.message || 'Failed to create loan offer'
    }
  } catch (error) {
    errorMessage.value = 'Error creating loan offer'
  }
}

onMounted(async () => {
  await loadLoanRequest()
})
</script>

<template>
  <div v-if="errorMessage">
    <h4 class="text-danger">{{ errorMessage }}</h4>
  </div>
  <RouterLink :to="`/loan-requests`" class="btn btn-secondary btn-sm mb-3"
    >Back to loan requests</RouterLink
  >
  <div class="container">
    <div v-if="loanRequest">
      <div class="loan-request-card">
        <div class="loan-request-header">
          <h6>
            <RouterLink
              :to="`/company/${loanRequest.company.id}`"
              class="btn btn-light btn"
              style="padding: 1px"
            >
              {{ loanRequest.company.companyName }}
            </RouterLink>
            is asking for:
          </h6>
          <h6 v-if="!loanRequest.active" class="text-danger">This loan request is no longer active!</h6>
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

      <button v-if="loanRequest.active" @click="showForm = !showForm" class="btn btn-sm btn-primary mb-3 mt-2">
        {{ showForm ? 'Hide form' : 'Make an offer' }}
      </button>

      <div v-if="showForm" class="form">
        <h4>Make an offer</h4>
        <form @submit.prevent="createLoanOffer">
          <div class="form-group">
            <label for="amount">Amount (€):</label>
            <input
              id="amount"
              v-model="newOffer.amount"
              type="number"
              step="0.01"
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="interest">Interest (%):</label>
            <input
              id="interest"
              v-model="newOffer.interest"
              type="number"
              step="0.01"
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="period">Period (months):</label>
            <input id="period" v-model="newOffer.period" type="number" class="form-control" />
          </div>
          <div class="form-group">
            <label for="comment">Comment:</label>
            <textarea id="comment" v-model="newOffer.comment" class="form-control"></textarea>
          </div>
          <button type="submit" class="btn btn-success">Submit Offer</button>
        </form>
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
          <LoanOfferCard :loanOffer="offer" :companyView="false" :refreshMethod="loadLoanRequest" :linkToRequest="false"/>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.detail-item {
  flex: 1 1 calc(50% - 12px);
  background-color: #fff;
  padding: 8px;
  border-radius: 4px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
  font-size: 0.9em;
}

.half-width {
  flex: 1 1 calc(50% - 12px);
}

.form {
  margin-top: 20px;
}

.form-group {
  margin-bottom: 1rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 0.5rem;
  box-sizing: border-box;
}

.btn {
  margin-top: 0rem;
}

.text-danger {
  color: red;
}
</style>

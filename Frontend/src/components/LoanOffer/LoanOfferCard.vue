<script setup lang="ts">
import { ref } from 'vue'
import type { ILoanOffer } from '@/domain/ILoanOffer'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { BaseService } from '@/services/BaseService'
import type { ILoan } from '@/domain/ILoan'

const authStore = useAuthStore()
const router = useRouter()

interface Props {
  linkToRequest: boolean;
  loanOffer: ILoanOffer
  role?: ECompanyRole
  companyView: boolean
  refreshMethod: () => void
}
const props = defineProps<Props>()
const showDetails = ref(false)

const deleteOffer = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.delete(`loanoffers/${props.loanOffer.id}`, jwt!)
    if (response.ok) {
      props.refreshMethod()
      if (!props.companyView) {
        props.refreshMethod()
      } else {
        props.refreshMethod()
      }
    } else {
      console.error('Failed to delete company:', response.message)
    }
  } catch (error) {
    console.error('Error deleting company:', error)
  }
}

const confirmDelete = () => {
  if (window.confirm('Are you sure you want to delete this loan offer?')) {
    deleteOffer()
  }
}

const acceptOffer = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.create<ILoan, ILoanOffer>(
      props.loanOffer,
      'loanoffers/acceptoffer',
      jwt!
    )
    if (response.ok) {
      router.push(`/company/${response.data!.companyId}/loan/${response.data!.id}`)
    } else {
      console.error('Failed to accept offer:', response.message)
    }
  } catch (error) {
    console.error('Error accepting offer:', error)
  }
}

const confirmAccept = () => {
  if (
    window.confirm(
      'Are you sure you want to accept this loan offer? This will create a new loan with the offered terms.'
    )
  ) {
    acceptOffer()
  }
}

const confirmDecline = () => {
  if (window.confirm('Are you sure you want to decline this loan offer?')) {
    deleteOffer()
  }
}
</script>

<template>
  <div class="loan-offer-card">
    <div class="loan-offer-header">
      <h6 v-if="companyView">{{ loanOffer.appUser!.email }} is offering:</h6>
      <h6 v-if="!companyView">Offer for {{ loanOffer.loanRequest?.company!.companyName }}:</h6>

    </div>
    <div class="loan-offer-summary">
      <div class="detail-item">Amount: €{{ loanOffer.amount.toFixed(2) }}</div>
      <div class="detail-item">Interest: {{ loanOffer.interest.toFixed(2) }}%</div>
      <div class="detail-item">Period: {{ loanOffer.period }} months</div>
    </div>
    <button @click="showDetails = !showDetails" class="btn btn-link">
      {{ showDetails ? 'Show less' : 'Show more' }}
    </button>
    <div v-if="showDetails" class="loan-offer-details">
      <div class="detail-item">
        Created: {{ new Date(loanOffer.createdAt).toLocaleDateString() }}
      </div>
      <div class="detail-item full-width">Comment: {{ loanOffer.comment }}</div>
      <div v-if="companyView && role === ECompanyRole.Manager">
        <button @click="confirmAccept" class="btn btn-sm btn-success">Accept</button>
      </div>
      <div v-if="companyView && role === ECompanyRole.Manager">
        <button @click="confirmDelete" class="btn btn-sm btn-danger">Decline</button>
      </div>
      <div v-if="!companyView && linkToRequest">
        <RouterLink :to="`/loan-request/${loanOffer.loanRequestId}`" class="btn btn-sm btn-primary">View request</RouterLink>
      </div>
      <div v-if="!companyView && loanOffer.appUserId === authStore.getUserId">
        <button @click="confirmDecline" class="btn btn-sm btn-danger">Delete</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.loan-offer-card {
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 8px;
  background-color: #f9f9f9;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.loan-offer-header h6 {
  margin: 0;
  font-size: 1.25em;
  color: #333;
}

.loan-offer-summary,
.loan-offer-details {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.detail-item {
  flex: 1 1 calc(33% - 12px);
  background-color: #fff;
  padding: 8px;
  border-radius: 4px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
  font-size: 0.9em;
}

.full-width {
  flex: 1 1 100%;
}

.btn-link {
  background: none;
  border: none;
  color: #007bff;
  padding: 0;
  cursor: pointer;
  font-size: 0.9em;
  margin-top: 10px;
}

.btn-link:hover {
  text-decoration: underline;
}
</style>

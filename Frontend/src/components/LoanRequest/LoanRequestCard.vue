<script setup lang="ts">
import type { ILoanRequest } from '@/domain/ILoanRequest'
import { onMounted, ref } from 'vue';

interface Props {
  loanRequest: ILoanRequest
  companyView: boolean
}
const props = defineProps<Props>()

</script>

<template>
  <div class="loan-request-card">
    <div class="loan-request-header">
      <h6 v-if="!companyView">{{ loanRequest.company.companyName }} is asking for:</h6>
      <h6 v-if="!loanRequest.active" class="text-danger">This loan request is no longer active!</h6>
    </div>
    <div class="loan-request-details">
      <div class="detail-item">Amount: €{{ loanRequest.amount.toFixed(2) }}</div>
      <div class="detail-item">Interest: {{ loanRequest.interest.toFixed(2) }}%</div>
      <div class="detail-item">
        Created: {{ new Date(loanRequest.createdAt).toLocaleDateString() }}
      </div>
      <div class="detail-item">Period: {{ loanRequest.period }} months</div>
      <div class="detail-item half-width">Offers: {{ loanRequest.loanOffers.length }}</div>
      <div v-if="companyView" class="loan-request-actions half-width">
        <router-link
          :to="`/company/${loanRequest.company.id}/loan-request/${loanRequest.id}`"
          class="btn btn-secondary full-width"
          >Details</router-link
        >
      </div>
      <div v-if="!companyView" class="loan-request-actions half-width">
        <router-link :to="`/loan-request/${loanRequest.id}`" class="btn btn-secondary full-width"
          >Details</router-link
        >
      </div>
    </div>
  </div>
</template>

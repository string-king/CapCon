<script setup lang="ts">
import type { ILoan } from '@/domain/ILoan'
import { ETransactionType } from '@/domain/enums/ETransactionType';
import { RouterLink } from 'vue-router'

interface Props {
  loan: ILoan
  companyView: boolean
}

const props = defineProps<Props>()

const getLoanStatus = () => {
  if (props.loan.transactions.length === 0) {
    return 'Waiting for lender to pay loan amount.'
  } else if (props.loan.transactions.length > 0 && props.loan.active) {
    return 'Active'
  } else if (props.loan.transactions.length > 0 && !props.loan.active) {
    return 'Completed!'
  } else {
    return 'Something went wrong...'
  }
}

const getLoanBalance = () => {
  if (!props.loan.active) {
    return 0
  } else {
    return props.loan.amount * (1 + props.loan.interest / 100) - props.loan.transactions
      .filter((t) => t.transactionType === ETransactionType.BorrowerToLender)
      .reduce((acc, t) => acc + t.amount, 0)
  }

}
</script>

<template>
  <div class="loan-request-card">
    <div class="loan-request-header">
      <h6 v-if="!companyView">Loaned out to {{ props.loan.company.companyName }}:</h6>
    </div>
    <div class="loan-request-header">
      <h6 v-if="companyView">Loan from {{ props.loan.appUser.email }}:</h6>
    </div>
    <div class="loan-request-details">
      <div class="detail-item">Amount: €{{ props.loan.amount.toFixed(2) }}</div>
      <div class="detail-item">Interest: {{ props.loan.interest.toFixed(2) }}%</div>
      <div class="detail-item" v-if="loan.transactions.length > 0">
        Start date: {{ new Date(props.loan.startDate).toLocaleDateString() }}
      </div>
      <div class="detail-item" v-if="loan.transactions.length === 0">
        Offer accepted at {{ new Date(props.loan.startDate).toLocaleDateString() }}
      </div>
      <div class="detail-item">Period: {{ props.loan.period }} months</div>
      <div class="detail-item half-width">Status: {{ getLoanStatus() }}</div>
      <div class="detail-item half-width">Loan balance: {{ getLoanBalance().toFixed(2) }}€</div>

      <div v-if="companyView" class="loan-request-actions half-width">
        <router-link
          :to="`/company/${props.loan.companyId}/loan/${props.loan.id}`"
          class="btn btn-secondary full-width"
          >Go to loan</router-link
        >
      </div>
      <div v-if="!companyView" class="loan-request-actions half-width">
        <RouterLink :to="`/loan/${props.loan.id}`" class="btn btn-secondary full-width"
          >Go to loan</RouterLink
        >
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import type { ITransaction } from '@/domain/ITransaction'
import { ETransactionType } from '@/domain/enums/ETransactionType'

interface Props {
  transaction: ITransaction
  companyView: boolean
  companyName: string
  userName: string
}
const props = defineProps<Props>()

const transaction = props.transaction

const getAmountPrefix = computed(() => {
  if (props.companyView) {
    return transaction.transactionType === ETransactionType.LenderToBorrower ? '+' : '-'
  }
  return transaction.transactionType === ETransactionType.LenderToBorrower ? '-' : '+'
})

const getAmountClass = computed(() => {
  return getAmountPrefix.value === '-' ? 'amount-negative' : 'amount-positive'
})

const from = computed(() => {
  return transaction.transactionType === ETransactionType.LenderToBorrower ? props.userName : props.companyName
})
</script>


<template>
  <div class="transaction-card">
    <div class="transaction-header">
      <h6>Transaction from {{ from }}</h6>
    </div>
    <div class="transaction-details">
      <div :class="['transaction-amount', getAmountClass]">
        {{ getAmountPrefix }}{{ transaction.amount }}
      </div>
      <div class="transaction-comment" v-if="transaction.comment.length>0">
        Comment: {{ transaction.comment }}
      </div>
      <div class="transaction-date">
        Date: {{ new Date(transaction.date).toLocaleDateString() }}
      </div>
    </div>
  </div>
</template>


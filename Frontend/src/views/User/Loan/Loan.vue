<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { BaseService } from '@/services/BaseService'
import { useAuthStore } from '@/stores/auth'
import type ICompany from '@/domain/ICompany'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import type { ILoan } from '@/domain/ILoan'
import { ETransactionType } from '@/domain/enums/ETransactionType'
import type { ITransaction } from '@/domain/ITransaction'
import TransactionCard from '@/components/Transaction/TransactionCard.vue'

const route = useRoute()
const loanId = route.params.id
const authStore = useAuthStore()

const loan = ref<ILoan>()
const errorMessage = ref('')
const loanEndDate = ref<Date>()

const showForm = ref(false)
const formErrorMessage = ref('')
const transactionAmount = ref(loan.value?.amount)
const transactionComment = ref('')

const getLoanStatus = computed(() => {
  if (loan.value?.transactions.length === 0) {
    return 'Waiting for lender to pay loan amount.'
  } else if (loan.value?.transactions.length! > 0 && loan.value?.active) {
    return 'Active'
  } else if (loan.value?.transactions.length! > 0 && !loan.value?.active) {
    return 'Completed!'
  } else {
    return 'Something went wrong...'
  }
})

const getLoanBalance = computed(() => {
  if (!loan.value?.active) {
    return 0
  } else {
    return (
      loan.value?.amount * (1 + loan.value?.interest / 100) -
      loan.value?.transactions
        .filter((t) => t.transactionType === ETransactionType.BorrowerToLender)
        .reduce((acc, t) => acc + t.amount, 0)
    )
  }
})

const loadCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ILoan>(`loans/${loanId}`, jwt!)
    if (response.ok) {
      loan.value = response.data!
      setProperties()
    } else {
      console.error('Failed to fetch company:', response.message)
      errorMessage.value = 'Failed to fetch company: ' + response.message
    }
  } catch (error) {
    console.error('Error loading company:', error)
    errorMessage.value = 'Error loading company: ' + error
  }
}

const setProperties = () => {
  if (!loan.value) {
    return
  }
  const loanStartDate = new Date(loan.value.startDate)
  loanEndDate.value = new Date(loanStartDate)
  loanEndDate.value.setMonth(loanStartDate.getMonth() + loan.value.period)
}

const addTransaction = async () => {
  try {
    const jwt = authStore.jwt
    const newTransaction: ITransaction = {
      loanId: loanId.toString(),
      amount: transactionAmount.value ?? 0,
      comment: transactionComment.value,
      transactionType: ETransactionType.LenderToBorrower,
      date: new Date()
    }
    const response = await BaseService.create<ITransaction>(newTransaction, `transactions`, jwt!)
    if (response.ok) {
      loadCompany()
      transactionAmount.value = 0
      transactionComment.value = ''
    } else {

      console.error('Failed to add transaction:', response.message)
      errorMessage.value = 'Failed to add transaction: ' + response.message
    }
  } catch (error) {
    console.error('Error adding transaction:', error)
    errorMessage.value = 'Error adding transaction: ' + error
  }
}

const validateNumberInput = (value: string) => {
  const numberValue = parseFloat(value)
  if (isNaN(numberValue) || numberValue <= 0 || !/^\d+(\.\d{1,2})?$/.test(value)) {
    formErrorMessage.value =
      'Amount and interest must be positive numbers with up to 2 decimal places.'
    return null
  }
  return numberValue
}

const validateForm = async () => {
  if (!transactionAmount.value) {
    formErrorMessage.value = 'Amount is required.'
    return
  }
  const amount = validateNumberInput(transactionAmount.value.toString())
  if (amount === null || isNaN(amount) || amount <= 0) {
    formErrorMessage.value = 'Amount must be a positive number.'
    return
  }
  if (loan.value?.transactions.length === 0 && amount !== loan.value?.amount) {
    formErrorMessage.value = 'First transaction must be equal to the loan amount.'
    return
  }

  await addTransaction()
  showForm.value = false
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
    <RouterLink :to="`/loans`" class="btn btn-secondary btn-sm mb-3">Back to loans</RouterLink>
    <div v-if="loan">
      <h3>Loan</h3>
      <hr />
      <div class="loan-request-card">
        <div class="loan-request-header">
          <h6>
            Loan between lender {{ loan?.appUser.email }} and borrower
            {{ loan?.company.companyName }}:
          </h6>
        </div>
        <div class="loan-request-details">
          <div class="detail-item">Amount: €{{ loan.amount?.toFixed(2) }}</div>
          <div class="detail-item">Interest: {{ loan.interest.toFixed(2) }}%</div>
          <div class="detail-item" v-if="loan.transactions.length > 0">
            Start date: {{ new Date(loan.startDate).toLocaleDateString() }}
          </div>
          <div class="detail-item" v-if="loan.transactions.length === 0">
            Offer accepted at {{ new Date(loan.startDate).toLocaleDateString() }}
          </div>
          <div class="detail-item">Period: {{ loan.period }} months</div>
          <div class="detail-item half-width">Status: {{ getLoanStatus }}</div>
          <div class="detail-item half-width">Loan balance: {{ getLoanBalance.toFixed(2) }}€</div>
        </div>
      </div>
    </div>
    <hr />
    <div v-if="loan">
      <h3>Transactions</h3>
      <div class="container" v-if="loan?.transactions.length === 0">
        <button @click="showForm = !showForm" class="btn btn-primary">
          {{ showForm ? 'Cancel' : 'Wire loan amount to borrower and start loan period!' }}
        </button>
        <form v-if="showForm" @submit.prevent="validateForm" class="add-company-form">
          <h6 class="text-danger">{{ formErrorMessage }}</h6>
          <div>
            <label for="companyName">Transaction amount: (€)</label>
            <input
              type="text"
              id="companyName"
              v-model="transactionAmount"
              required
              minlength="1"
            />
          </div>
          <div>
            <label for="transactionComment">Comment</label>
            <input type="text" id="companyName" v-model="transactionComment" />
          </div>
          <button type="submit" class="btn btn-success">Add transaction</button>
        </form>
      </div>
      <hr />
      <div
        v-for="transaction in loan.transactions.sort(
          (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()
        )"
        v-bind:key="transaction.id"
      >
        <TransactionCard
          :transaction="transaction"
          :companyView="false"
          :companyName="loan.company.companyName"
          :userName="loan.appUser.email"
        />
      </div>
    </div>
  </div>
</template>

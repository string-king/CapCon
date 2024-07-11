<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { BaseService } from '@/services/BaseService'
import { useAuthStore } from '@/stores/auth'
import type ICompany from '@/domain/ICompany'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import type { ILoan } from '@/domain/ILoan'
import type { ITransaction } from '@/domain/ITransaction'
import { ETransactionType } from '@/domain/enums/ETransactionType'
import TransactionCard from '@/components/Transaction/TransactionCard.vue'

const route = useRoute()
const companyId = route.params.id
const loanId = route.params.loanId
const authStore = useAuthStore()

const company = ref<ICompany>()
const loan = ref<ILoan>()
const userRole = ref<ECompanyRole>(ECompanyRole.Viewer)
const errorMessage = ref('')
const formErrorMessage = ref('')
const loanEndDate = ref<Date>()

const showForm = ref(false)
const transactionAmount = ref<number | null>(null)
const transactionComment = ref('')

const loadCompany = async () => {
  errorMessage.value = ''
  formErrorMessage.value = ''
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ICompany>(`companies/${companyId}`, jwt!)
    if (response.ok) {
      company.value = response.data
      loan.value = company.value?.loans.find((l) => l.id === loanId)
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

const setUserRole = () => {
  if (!company.value || !authStore.isAuthenticated) {
    return
  }
  const userCompany = company.value.userCompanies.find((uc) => uc.appUserId === authStore.userId)
  if (userCompany) {
    userRole.value = userCompany.role
  }
}

const setProperties = () => {
  if (!loan.value) {
    return
  }
  setUserRole()
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
      transactionType: ETransactionType.BorrowerToLender,
      date: new Date()
    }
    const response = await BaseService.create<ITransaction>(newTransaction, `transactions`, jwt!)
    if (response.ok) {
      loadCompany()
      transactionAmount.value = 0
      transactionComment.value = ''
    } else {
      console.error('Failed to add transaction:', JSON.stringify(response))
      formErrorMessage.value = 'Failed to add transaction: ' + response.message
    }
  } catch (error) {
    console.error('Error adding transaction:', error)
    formErrorMessage.value = 'Error adding transaction: ' + error
  }
}

const validateNumberInput = (value: string) => {
  const normalizedValue = value.replace(',', '.')
  const numberValue = parseFloat(normalizedValue)
  if (isNaN(numberValue) || numberValue <= 0 || !/^\d+(\.\d{1,2})?$/.test(normalizedValue)) {
    formErrorMessage.value =
      'Amount and interest must be positive numbers with up to 2 decimal places.'
    return null
  }
  return numberValue
}

const validateForm = async () => {
  formErrorMessage.value = ''
  if (!transactionAmount.value) {
    formErrorMessage.value = 'Amount is required.'
    return
  }
  const amount = validateNumberInput(transactionAmount.value.toString())
  if (amount === null || isNaN(amount) || amount <= 0) {
    formErrorMessage.value = 'Amount must be a positive number.'
    return
  }
  if (amount > getLoanBalance.value) {
    formErrorMessage.value = 'Amount cannot be higher than the loan balance.'
    return
  }

  await addTransaction()
  showForm.value = false
}

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

onMounted(async () => {
  await loadCompany()
})
</script>

<template>
  <div class="container">
    <div v-if="errorMessage">
      <h4 class="text-danger">{{ errorMessage }}</h4>
    </div>
    <RouterLink :to="`/company/${company?.id}/loans`" class="btn btn-secondary btn-sm mb-3"
      >Back to loans</RouterLink
    >
    <div v-if="company && loan">
      <h3>{{ company.companyName }}'s loan</h3>
      <hr />
      <div class="loan-request-card">
        <div class="loan-request-header">
          <h6>
            Loan between lender {{ loan?.appUser.email }} and borrower
            {{ loan?.company.companyName }}:
          </h6>
        </div>
        <div class="loan-request-details">
          <div class="detail-item">Amount: €{{ loan.amount.toFixed(2) }}</div>
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
    <div v-if="company && loan">
      <h3>Transactions</h3>
      <div
        v-for="transaction in loan.transactions.sort(
          (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()
        )"
        v-bind:key="transaction.id"
      >
        <TransactionCard
          :transaction="transaction"
          companyView
          :companyName="company.companyName"
          :userName="loan.appUser.email"
        />
      </div>
    </div>
    <div
      class="container"
      v-if="userRole === ECompanyRole.Manager && loan?.transactions?.length! > 0"
    >
      <button v-if="loan?.active" @click="showForm = !showForm" class="btn btn-primary">
        {{ showForm ? 'Cancel' : 'Add transaction' }}
      </button>
      <form v-if="showForm" @submit.prevent="validateForm" class="add-company-form">
        <div v-if="formErrorMessage">
          <h5 class="text-danger">{{ formErrorMessage }}</h5>
        </div>
        <div>
          <label for="transactionAmount">Transaction amount: (€)</label>
          <input
            type="number"
            id="transactionAmount"
            v-model="transactionAmount"
            required
            min="0.01"
            step="0.01"
          />
        </div>
        <div>
          <label for="transactionComment">Comment</label>
          <input type="text" id="transactionComment" v-model="transactionComment" />
        </div>
        <button type="submit" class="btn btn-success">Add transaction</button>
      </form>
    </div>
  </div>
</template>

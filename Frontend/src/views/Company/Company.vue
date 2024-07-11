<script lang="ts" setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type ICompany from '@/domain/ICompany'
import { BaseService } from '@/services/BaseService'
import type { ILoanRequest } from '@/domain/ILoanRequest'
import type { ILoan } from '@/domain/ILoan'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'

const route = useRoute()
const router = useRouter()
const companyId = route.params.id
const authStore = useAuthStore()

const company = ref<ICompany | null>(null)
const userRole = ref<ECompanyRole>(ECompanyRole.Viewer)
const errorMessage = ref('')
const activeLoanRequests = ref<ILoanRequest[]>([])
const activeLoans = ref<ILoan[]>([])
const pendingLoans = ref<ILoan[]>([])
const completedLoans = ref<ILoan[]>([])
const averageLoanAmount = ref<number>(0)
const averageRequestAmount = ref<number>(0)
const averageRequestInterest = ref<number>(0)
const averageRequestPeriod = ref<number>(0)

const loadCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ICompany>(`companies/${companyId}`, jwt!)
    if (response.ok && response.data) {
      company.value = response.data
      categorizeLoans()
      calculateAverageValues()
      setUserRole()
    } else {
      errorMessage.value = `Failed to fetch company: ${response.message}`
    }
  } catch (error) {
    errorMessage.value = `Error loading company: ${error}`
  }
}

const deleteCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.delete(`companies/${companyId}`, jwt!)
    if (response.ok) {
      router.push('/companies')
    } else {
      errorMessage.value = `Failed to delete company: ${response.message}`
    }
  } catch (error) {
    errorMessage.value = `Error deleting company: ${error}`
  }
}

const confirmDelete = () => {
  if (window.confirm('Are you sure you want to delete this company? This will also delete all associated loans, loan requests, and loan offers.')) {
    deleteCompany()
  }
}

const setUserRole = () => {
  if (!company.value || !authStore.isAuthenticated) return

  const userCompany = company.value.userCompanies.find(uc => uc.appUserId === authStore.userId)
  if (userCompany) userRole.value = userCompany.role
}

const categorizeLoans = () => {
  if (!company.value) return

  activeLoans.value = company.value.loans.filter(loan => loan.active)
  pendingLoans.value = company.value.loans.filter(loan => !loan.active && loan.transactions.length === 0)
  completedLoans.value = company.value.loans.filter(loan => !loan.active && loan.transactions.length > 0)
}

const calculateAverageValues = () => {
  if (!company.value) return

  activeLoanRequests.value = company.value.loanRequests.filter(lr => lr.active)
  if (activeLoanRequests.value.length > 0) {
    averageRequestAmount.value = activeLoanRequests.value.reduce((acc, lr) => acc + lr.amount, 0) / activeLoanRequests.value.length
    averageRequestInterest.value = activeLoanRequests.value.reduce((acc, lr) => acc + lr.interest, 0) / activeLoanRequests.value.length
    averageRequestPeriod.value = activeLoanRequests.value.reduce((acc, lr) => acc + lr.period, 0) / activeLoanRequests.value.length
  }

  if (activeLoans.value.length > 0) {
    averageLoanAmount.value = activeLoans.value.reduce((acc, loan) => acc + loan.amount, 0) / activeLoans.value.length
  }
}

const averageLoanInterest = computed(() => {
  if (!activeLoans.value.length) return 0
  const totalInterest = activeLoans.value.reduce((total, loan) => total + loan.interest, 0)
  return (totalInterest / activeLoans.value.length).toFixed(2)
})

const averageLoanPeriod = computed(() => {
  if (!activeLoans.value.length) return 0
  const totalPeriod = activeLoans.value.reduce((total, loan) => total + loan.period, 0)
  return Math.round(totalPeriod / activeLoans.value.length)
})

const averagePendingLoanAmount = computed(() => {
  if (!pendingLoans.value.length) return 0
  const totalAmount = pendingLoans.value.reduce((total, loan) => total + loan.amount, 0)
  return (totalAmount / pendingLoans.value.length).toFixed(2)
})

const averageCompletedLoanAmount = computed(() => {
  if (!completedLoans.value.length) return 0
  const totalAmount = completedLoans.value.reduce((total, loan) => total + loan.amount, 0)
  return (totalAmount / completedLoans.value.length).toFixed(2)
})

const averageCompletedLoanInterest = computed(() => {
  if (!completedLoans.value.length) return 0
  const totalInterest = completedLoans.value.reduce((total, loan) => total + loan.interest, 0)
  return (totalInterest / completedLoans.value.length).toFixed(2)
})

const averageCompletedLoanPeriod = computed(() => {
  if (!completedLoans.value.length) return 0
  const totalPeriod = completedLoans.value.reduce((total, loan) => total + loan.period, 0)
  return Math.round(totalPeriod / completedLoans.value.length)
})

onMounted(loadCompany)
</script>


<template>
  <div class="container">
    <RouterLink :to="`/companies`" class="btn btn-secondary btn-sm mb-3">
      Back to my companies
    </RouterLink>

    <div v-if="errorMessage">
      <h4 class="text-danger">{{ errorMessage }}</h4>
    </div>

    <div v-if="company">
      <h1>{{ company.companyName }}</h1>
      <hr />

      <!-- Loans Section -->
      <div>
        <h3>Loans</h3>
        <RouterLink
          v-if="company.loans.length > 0"
          :to="`/company/${company.id}/loans`"
          class="btn btn-primary btn-sm"
        >
          View all loans
        </RouterLink>

        <div v-if="activeLoans.length > 0" class="card mt-3">
          <div class="card-body">
            <h5>Active Loans Overview</h5>
            <p class="card-text">
              <b>{{ company.companyName }}</b> currently has <b>{{ activeLoans.length }}</b> active loans. The average loan amount is <b>{{ averageLoanAmount }} €</b>, with an average interest rate of <b>{{ averageLoanInterest }}%</b>, and an average period of <b>{{ averageLoanPeriod }} months</b>.
            </p>
          </div>
        </div>

        <div v-if="pendingLoans.length > 0" class="card mt-3">
          <div class="card-body">
            <h5>Pending Loans Overview</h5>
            <p class="card-text">
              <b>{{ company.companyName }}</b> has <b>{{ pendingLoans.length }}</b> pending loans awaiting transactions. The average pending loan amount is <b>{{ averagePendingLoanAmount }} €</b>.
            </p>
          </div>
        </div>

        <div v-if="completedLoans.length > 0" class="card mt-3">
          <div class="card-body">
            <h5>Completed Loans Overview</h5>
            <p class="card-text">
              <b>{{ company.companyName }}</b> has successfully completed <b>{{ completedLoans.length }}</b> loans. The average completed loan amount was <b>{{ averageCompletedLoanAmount }} €</b>, with an average interest rate of <b>{{ averageCompletedLoanInterest }}%</b>, and an average period of <b>{{ averageCompletedLoanPeriod }} months</b>.
            </p>
          </div>
        </div>
      </div>
      <hr />

      <!-- Loan Requests Section -->
      <div>
        <h3>Loan Requests</h3>
        <RouterLink
          v-if="userRole === ECompanyRole.Manager"
          :to="`/company/${company.id}/loan-requests/create`"
          class="btn btn-primary btn-sm"
          style="margin-right: 10px"
        >
          Add a loan request
        </RouterLink>
        <RouterLink
          v-if="company.loanRequests.length > 0"
          :to="`/company/${company.id}/loan-requests`"
          class="btn btn-primary btn-sm"
        >
          View all requests
        </RouterLink>
        <div v-if="company.loanRequests.length > 0" class="card mt-3">
          <div class="card-body">
            <h5>Loan Requests Overview</h5>
            <p class="card-text">
              <b>{{ company.companyName }}</b> currently has <b>{{ activeLoanRequests.length }}</b> active loan requests. The average requested amount is <b>{{ averageRequestAmount }} €</b>, with an average interest rate of <b>{{ averageRequestInterest }}%</b>, and an average period of <b>{{ averageRequestPeriod }} months</b>.
            </p>
          </div>
        </div>
      </div>
      <hr />

      <!-- Members Section -->
      <div>
        <h3>Members</h3>
        <div v-if="userRole === ECompanyRole.Manager">
          <RouterLink :to="`/company/${company.id}/members`" class="btn btn-primary btn-sm">
            Manage members
          </RouterLink>
        </div>
        <br />
        <h5>Managers:</h5>
        <ul>
          <li
            v-for="userCompany in company.userCompanies.filter(uc => uc.role === ECompanyRole.Manager)"
            :key="userCompany.id"
          >
            {{ userCompany.appUser.firstName }} {{ userCompany.appUser.lastName }},
            {{ userCompany.appUser.email }}
          </li>
        </ul>
        <div v-if="company.userCompanies.find(uc => uc.role === ECompanyRole.Viewer)">
          <br />
          <h5>Viewers:</h5>
          <ul>
            <li
              v-for="userCompany in company.userCompanies.filter(uc => uc.role === ECompanyRole.Viewer)"
              :key="userCompany.id"
            >
              {{ userCompany.appUser.firstName }} {{ userCompany.appUser.lastName }},
              {{ userCompany.appUser.email }}
            </li>
          </ul>
        </div>
      </div>

      <!-- Delete Company Button -->
      <div v-if="userRole === ECompanyRole.Manager">
        <hr />
        <button @click="confirmDelete" class="btn btn-danger btn-sm">Delete company</button>
      </div>
    </div>
  </div>
</template>

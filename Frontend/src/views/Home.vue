<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { IHomePageData } from '@/domain/IHomePageData'
import { BaseService } from '@/services/BaseService'

const authStore = useAuthStore()
const homePageData = ref<IHomePageData>()
const errorMessage = ref('')
const totalLoanAmount = ref(0)
const averageLoanAmount = ref(0)
const averageLoanInterest = ref(0)
const averageLoanPeriod = ref(0)
const totalLoanOfferAmount = ref(0)
const averageLoanOfferAmount = ref(0)
const averageLoanOfferInterest = ref(0)
const averageLoanOfferPeriod = ref(0)

const calculateValues = () => {
  totalLoanAmount.value = homePageData.value!.userLoans.reduce(
    (total, loan) => total + loan.amount,
    0
  )
  averageLoanAmount.value = totalLoanAmount.value / homePageData.value!.userLoans.length

  averageLoanInterest.value =
    homePageData.value!.userLoans.reduce((total, loan) => total + loan.interest, 0) /
    homePageData.value!.userLoans.length

  averageLoanPeriod.value =
    homePageData.value!.userLoans.reduce((total, loan) => total + loan.period, 0) /
    homePageData.value!.userLoans.length

  totalLoanOfferAmount.value = homePageData.value!.userLoanOffers.reduce(
    (total, loanOffer) => total + loanOffer.amount,
    0
  )
  averageLoanOfferAmount.value =
    totalLoanOfferAmount.value / homePageData.value!.userLoanOffers.length

  averageLoanOfferInterest.value =
    homePageData.value!.userLoanOffers.reduce((total, loanOffer) => total + loanOffer.interest, 0) /
    homePageData.value!.userLoanOffers.length

  averageLoanOfferPeriod.value =
    homePageData.value!.userLoanOffers.reduce((total, loanOffer) => total + loanOffer.period, 0) /
    homePageData.value!.userLoanOffers.length
}

const loadHomePageData = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<IHomePageData>('home', jwt!)
    if (response.ok && response.data) {
      homePageData.value = response.data
      calculateValues()
    } else {
      errorMessage.value = `Failed to fetch home page data: ${response.message}`
    }
  } catch (error) {
    errorMessage.value = `Error loading home page data: ${error}`
  }
}

onMounted(() => {
  loadHomePageData()
})
</script>

<template>
  <div class="container">
    <h3>Welcome, {{ authStore.userName }}!</h3>
    <p>
      Here you can get a brief overview of your companies, loan offers, and loans. Let's get you up to date!
    </p>
    <div v-if="errorMessage">
      <h4 class="text-danger">{{ errorMessage }}</h4>
    </div>

    <!-- Loans Overview -->
    <div v-if="homePageData">
      <div class="card mb-3">
        <div class="card-body">
          <h5 class="card-title">My Loans</h5>
          <p class="card-text">
            You have issued a total of <b>{{ homePageData.userLoans.length }}</b> loans, of which <b>{{ homePageData.userLoans.filter((l) => l.active).length }}</b> are currently active.
            <span v-if="homePageData.userLoans.length > 0">
              The total amount of money you have loaned out is <b>{{ totalLoanAmount.toFixed(2) }}€</b>. On average, each loan amount is <b>{{ averageLoanAmount.toFixed(2) }}€</b>, with an average interest rate of <b>{{ averageLoanInterest.toFixed(2) }}%</b> over a period of <b>{{ averageLoanPeriod.toFixed(0) }} months</b>.
            </span>
          </p>
          <RouterLink to="/loans" class="btn btn-primary">View My Loans</RouterLink>
        </div>
      </div>
    </div>

    <!-- Loan Offers Overview -->
    <div v-if="homePageData">
      <div class="card mb-3">
        <div class="card-body">
          <h5 class="card-title">My Loan Offers</h5>
          <p class="card-text">
            You have made <b>{{ homePageData.userLoanOffers.length }}</b> loan offers, of which <b>{{ homePageData.userLoanOffers.filter((l) => l.active).length }}</b> are currently active.
            <span v-if="homePageData.userLoans.length > 0">
              The average amount for each of your loan offers is <b>{{ averageLoanOfferAmount.toFixed(2) }}€</b>, with an average interest rate of <b>{{ averageLoanOfferInterest.toFixed(2) }}%</b> over a period of <b>{{ averageLoanOfferPeriod.toFixed(0) }} months</b>.
            </span>
          </p>
          <RouterLink to="/loan-offers" class="btn btn-primary">View My Loan Offers</RouterLink>
        </div>
      </div>
    </div>

    <!-- Companies Overview -->
    <div v-if="homePageData">
      <div class="card mb-3">
        <div class="card-body">
          <h5 class="card-title">My Companies</h5>
          <p class="card-text">
            You are a member of <b>{{ homePageData.userCompanies.length }}</b> 
            <span v-if="homePageData.userCompanies.length === 0">
              companies. If you want to start creating loan requests, you need to create a company first.
            </span>
            <span v-if="homePageData.userCompanies.length === 1">
              company. Check it out to see if there are any new activities related to loan requests or loans.
            </span>
            <span v-if="homePageData.userCompanies.length > 1">
              companies. Check them out to see if there are any new activities related to loan requests or loans.
            </span>
          </p>
          <RouterLink to="/companies" class="btn btn-primary">View My Companies</RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

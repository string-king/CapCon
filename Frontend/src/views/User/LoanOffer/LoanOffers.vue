<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { BaseService } from '@/services/BaseService'
import { useAuthStore } from '@/stores/auth'
import type {ILoanOffer} from '@/domain/ILoanOffer'
import LoanOfferCard from '@/components/LoanOffer/LoanOfferCard.vue'

const loanOffers = ref<ILoanOffer[]>()
const authStore = useAuthStore()

const loadLoanOffers = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ILoanOffer[]>('loanoffers', jwt!)
    if (response.ok) {
      loanOffers.value = response.data
    } 
    else {
      console.error('Failed to fetch loan offers:', response.message)
      errorMessage.value = 'Failed to fetch loan offers: ' + response.message
    }
  } catch (error) {
    console.error('Error loading loan offers:', error)
    errorMessage.value = 'Error loading loan offers: ' + error
  }
}

const errorMessage = ref('')

onMounted(async () => {
  await loadLoanOffers()
})

</script>

<template>
  <div class="container">
    <div v-if="errorMessage">
      <h4 class="text-danger">{{ errorMessage }}</h4>
    </div>
    <h2>Your loan offers</h2>
    <p>
      Here you can view and edit your loan offers.
      <b
        >NB! When your loan offer gets accepted you have to make the first transaction and wire the
        loan amount to the borrower!</b
      >
    </p>
    <hr />
    <div v-if="loanOffers?.length === 0">
      No active loan offers, create one now!
    </div>
    <div v-for="loanOffer in loanOffers" v-bind:key="loanOffer.id">
      <LoanOfferCard :loanOffer="loanOffer" :company-view="false" :refresh-method="loadLoanOffers" :link-to-request="true"/>
    </div>
  </div>
</template>

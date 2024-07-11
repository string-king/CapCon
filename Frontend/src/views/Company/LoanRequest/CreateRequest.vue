<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { BaseService } from '@/services/BaseService'
import { useRoute, useRouter } from 'vue-router'
import type { ILoanRequestSimple } from '@/domain/ILoanRequestSimple'

const router = useRouter()
const route = useRoute()
const companyId = route.params.id as string

const authStore = useAuthStore()

const loanRequest = ref<ILoanRequestSimple>({
  companyId,
  amount: 0,
  interest: 0,
  period: 0,
  comment: '',
  active: true,
  createdAt: new Date(),
})

const errorMessage = ref('')
const amountInput = ref<string>('0');
const interestInput = ref<string>('0');

const validateNumberInput = (value: string) => {
  const normalizedValue = value.replace(',', '.');
  const numberValue = parseFloat(normalizedValue);
  if (isNaN(numberValue) || numberValue <= 0 || !/^\d+(\.\d{1,2})?$/.test(normalizedValue)) {
    errorMessage.value = 'Amount and interest must be positive numbers with up to 2 decimal places.';
    return null;
  }
  return numberValue;
};

const handleAmountInput = (event: Event) => {
  const input = event.target as HTMLInputElement;
  amountInput.value = input.value;
};

const handleInterestInput = (event: Event) => {
  const input = event.target as HTMLInputElement;
  interestInput.value = input.value;
};

const validateLoanRequest = (): boolean => {
  const amount = validateNumberInput(amountInput.value);
  const interest = validateNumberInput(interestInput.value);
  const period = parseInt(loanRequest.value.period.toString(), 10);

  if (amount === null || interest === null || isNaN(period) || period <= 0) {
    console.error('Validation failed: amount and interest must be positive numbers with up to 2 decimal places, and period must be a positive integer.');
    return false;
  }

  loanRequest.value.amount = amount;
  loanRequest.value.interest = interest;
  loanRequest.value.period = period;
  return true;
};

const createLoanRequest = async () => {
  errorMessage.value = '';
  if (!validateLoanRequest()) {
    return;
  }

  try {
    const jwt = authStore.jwt;
    if (!jwt) {
      throw new Error('Authentication token is missing');
    }

    const response = await BaseService.create<ILoanRequestSimple>(loanRequest.value, 'loanrequests', jwt);

    if (response.ok) {
      router.push(`/company/${companyId}`);
    } else {
      console.error('Failed to add company:', response.message);
    }
  } catch (error) {
    console.error('Error adding company:', error);
  }
};
</script>

<template>
  <div class="container">
    <RouterLink :to="`/company/${companyId}`" class="btn btn-secondary btn-sm mb-3"
      >Back to company</RouterLink
    >
    <h5 v-if="errorMessage" class="text-danger">{{ errorMessage }}</h5>
    <h1>Loan Request Form</h1>
    <form @submit.prevent="createLoanRequest" class="add-company-form">
      <div class="form-group">
        <label for="amount">Amount (€)</label>
        <input
          type="text"
          id="amount"
          v-model="amountInput"
          class="form-control"
          @input="handleAmountInput"
          required
        />
      </div>
      <div class="form-group">
        <label for="interest">Interest (%)</label>
        <input
          type="text"
          id="interest"
          v-model="interestInput"
          class="form-control"
          @input="handleInterestInput"
          required
        />
      </div>
      <div class="form-group">
        <label for="period">Period (months)</label>
        <input
          type="number"
          id="period"
          v-model.number="loanRequest.period"
          class="form-control"
          min="1"
          step="1"
          required
        />
      </div>
      <div class="form-group">
        <label for="comment">Comment</label>
        <textarea
          id="comment"
          v-model="loanRequest.comment"
          class="form-control"
          rows="3"
        ></textarea>
      </div>
      <button type="submit" class="btn btn-primary">Create request</button>
    </form>
  </div>
</template>

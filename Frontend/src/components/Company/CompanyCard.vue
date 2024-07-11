<script setup lang="ts">
import type ICompany from '@/domain/ICompany'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import { computed } from 'vue'

interface Props {
  company: ICompany
  role: ECompanyRole
}

const props = defineProps<Props>()

const roleLabel = computed(() => {
  return props.role === ECompanyRole.Manager ? 'Manager' : 'Viewer'
})
</script>

<template>
  <div class="company-card">
    <div class="company-info">
      <h5>{{ company.companyName }}</h5>
      <div class="company-details">
        <div class="detail-item">Loans: {{ company.loans.length }}</div>
        <div class="detail-item">Active loan requests: {{ company.loanRequests.filter(lr => lr.active).length }}</div>
        <div class="detail-item">Members: {{ company.userCompanies.length }}</div>
        <div class="half-width">
          <div class="detail-item">Role: {{ roleLabel }}</div>
        </div>
        <div class="half-width">
          <router-link :to="`/company/${company.id}`" class="btn btn-secondary full-width"
            >Go to company</router-link
          >
        </div>
      </div>
    </div>
  </div>
</template>

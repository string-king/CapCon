<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { BaseService } from '@/services/BaseService';
import CompanyCard from '@/components/Company/CompanyCard.vue';
import type { IUserCompany } from '@/domain/IUserCompany';
import type ICompany from '@/domain/ICompany';
import type ICompanySimple from '@/domain/ICompanySimple';

const userCompanies = ref<IUserCompany[]>();
const authStore = useAuthStore();

const showForm = ref(false);
const newCompanyName = ref('');

const loadUserCompanies = async () => {
  try {
    const jwt = authStore.jwt;
    if (!jwt) {
      throw new Error('Authentication token is missing');
    }

    const response = await BaseService.getAll<IUserCompany>('usercompanies', jwt);

    if (response.ok) {
      userCompanies.value = response.data || [];
    } else {
      if (response.statusCode === 401) {
        authStore.logout();
      }
      console.error('Failed to fetch companies:', response.message);
    }
  } catch (error) {
    console.error('Error loading user companies:', error);
  }
};

const addCompany = async () => {
  if (newCompanyName.value.length < 1) {
    alert('Company name must be at least 1 character long.');
    return;
  }

  const newCompany: ICompanySimple = { 
    companyName: newCompanyName.value };

  try {
    const jwt = authStore.jwt;
    if (!jwt) {
      throw new Error('Authentication token is missing');
    }

    const response = await BaseService.create<ICompany, ICompanySimple>(newCompany, 'companies', jwt);

    if (response.ok) {
      loadUserCompanies(); // Refresh the company list
      newCompanyName.value = '';
      showForm.value = false;
    } else {
      console.error('Failed to add company:', response.message);
    }
  } catch (error) {
    console.error('Error adding company:', error);
  }
};

onMounted(() => {
  loadUserCompanies();
});
</script>



<template>
  <div class="container">
    <h2>Manage, view or add your companies</h2>
    <p>
      Here you can view the companies you are associated with, and add new companies to your
      profile.
    </p>
    <div class="container">
      <button @click="showForm = !showForm" class="btn btn-primary">
        {{ showForm ? 'Cancel' : 'Add a company' }}
      </button>
      <form v-if="showForm" @submit.prevent="addCompany" class="add-company-form">
        <div>
          <label for="companyName">Company Name</label>
          <input 
            type="text" 
            id="companyName" 
            v-model="newCompanyName" 
            required 
            minlength="1" 
          />
        </div>
        <button type="submit" class="btn btn-success">Add Company</button>
      </form>
    </div>
    <hr />
    <div class="container">
      <div v-for="userCompany in userCompanies" :key="userCompany.id" class="container" style="padding: 5px;">
        <CompanyCard :company="userCompany.company" :role="userCompany.role" />
      </div>
    </div>
  </div>
</template>



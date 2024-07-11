<script lang="ts" setup>
import type ICompany from '@/domain/ICompany'
import { BaseService } from '@/services/BaseService'
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ECompanyRole } from '@/domain/enums/ECompanyRole'
import type { IUserCompany } from '@/domain/IUserCompany'
import type { INewMember } from '@/domain/INewMember'

const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()
const companyId = route.params.id

const company = ref<ICompany>()
const userRole = ref<ECompanyRole>(ECompanyRole.Viewer)
const showForm = ref(false)
const newMemberEmail = ref('')
const newMemberRole = ref<ECompanyRole>(ECompanyRole.Viewer)

const errorMessage = ref('')

const loadCompany = async () => {
  try {
    const jwt = authStore.jwt
    const response = await BaseService.get<ICompany>(`companies/${companyId}`, jwt!)
    if (response.ok) {
      company.value = response.data
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

const addMember = async () => {
  try {
    const jwt = authStore.jwt
    if (!jwt) {
      throw new Error('Authentication token is missing')
    }

    const newMember: INewMember = {
      companyId: companyId.toString(),
      email: newMemberEmail.value,
      role: newMemberRole.value
    }

    const response = await BaseService.create<IUserCompany, INewMember>(
      newMember,
      'usercompanies/addnewmember',
      jwt
    )
    if (response.ok) {
      await loadCompany() // Refresh company data
      showForm.value = false // Hide form after successful submission
      newMemberEmail.value = '' // Reset the form
      newMemberRole.value = ECompanyRole.Viewer // Reset the role selection
    } else {
      console.error('Failed to add member:', response.message)
      errorMessage.value = 'Failed to add member: ' + response.message
    }
  } catch (error) {
    console.error('Error adding member:', error)
    errorMessage.value = 'Error adding member: ' + error
  }
}

const removeMember = async (userCompanyId: string) => {
  try {
    const jwt = authStore.jwt
    if (!jwt) {
      throw new Error('Authentication token is missing')
    }

    const response = await BaseService.delete(`usercompanies/${userCompanyId}`, jwt)
    if (response.ok) {
      await loadCompany() // Refresh company data
    } else {
      console.error('Failed to remove member:', response.message)
      errorMessage.value = 'Failed to remove member: ' + response.message
    }
  } catch (error) {
    console.error('Error removing member:', error)
    errorMessage.value = 'Error removing member: ' + error
  }
}

const changeMemberRole = async (member: IUserCompany) => {
  try {
    const jwt = authStore.jwt
    if (!jwt) {
      throw new Error('Authentication token is missing')
    }

    const newRole =
      member.role === ECompanyRole.Manager ? ECompanyRole.Viewer : ECompanyRole.Manager
    const updatedMember = { ...member, role: newRole, companyId: companyId.toString() }

    const response = await BaseService.edit<IUserCompany>(
      updatedMember,
      `usercompanies/${member.id}`,
      jwt
    )
    if (response.ok) {
      await loadCompany() // Refresh company data
    } else {
      console.error('Failed to change member role:', response.message)
      errorMessage.value = 'Failed to change member role: ' + response.message
    }
  } catch (error) {
    console.error('Error changing member role:', error)
    errorMessage.value = 'Error changing member role: ' + error
  }
}

onMounted(async () => {
  await loadCompany()
  setUserRole()
})
</script>

<template>
  <div v-if="errorMessage">
    <h4 class="text-danger">{{ errorMessage }}</h4>
  </div>
  <div v-if="company" class="container">
    <RouterLink :to="`/company/${companyId}`" class="btn btn-secondary btn-sm mb-3"
      >Back to company</RouterLink
    >
    <h1>Members of {{ company?.companyName }}</h1>
    <hr />
    <button @click="showForm = !showForm" class="btn btn-primary">
      {{ showForm ? 'Cancel' : 'Add a member' }}
    </button>
    <form
      v-if="showForm && userRole === ECompanyRole.Manager"
      @submit.prevent="addMember"
      class="add-company-form"
    >
      <div class="form-group">
        <label for="newMemberEmail">Email</label>
        <input
          type="email"
          id="newMemberEmail"
          v-model="newMemberEmail"
          class="form-control"
          required
          minlength="1"
        />
      </div>
      <div class="form-group">
        <label for="newMemberRole">Role</label>
        <select id="newMemberRole" v-model="newMemberRole" class="form-control" required>
          <option :value="ECompanyRole.Manager">Manager</option>
          <option :value="ECompanyRole.Viewer">Viewer</option>
        </select>
      </div>
      <button type="submit" class="btn btn-success">Add Member</button>
    </form>
    <br />
    <br />
    <table v-if="company" class="table">
      <thead>
        <tr>
          <th>Email</th>
          <th>Role</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="member in company.userCompanies" :key="member.appUserId">
          <td>{{ member.appUser?.email }}</td>
          <td>{{ member.role === 0 ? 'Manager' : 'Viewer' }}</td>
          <td>
            <button
              v-if="member.appUser?.id !== authStore.userId && userRole === ECompanyRole.Manager"
              @click="removeMember(member.id)"
              class="btn btn-danger"
              style="margin-right: 10px;"
            >
              Remove
            </button>
            <button
              v-if="member.appUser?.id !== authStore.userId && userRole === ECompanyRole.Manager"
              @click="changeMemberRole(member)"
              class="btn btn-secondary"
            >
              Make {{ member.role === 1 ? 'manager' : 'viewer' }}
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

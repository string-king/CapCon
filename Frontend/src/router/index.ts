import Login from '@/views/Identity/Login.vue'
import Register from '@/views/Identity/Register.vue'
import Home from '@/views/Home.vue'
import Index from '@/views/Index.vue'
import Companies from '@/views/User/Companies.vue'
import CreateRequest from '@/views/Company/LoanRequest/CreateRequest.vue'
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import Company from '@/views/Company/Company.vue'
import CompanyMembers from '@/views/Company/CompanyMembers.vue'
import  CompanyRequests from '@/views/Company/LoanRequest/CompanyRequests.vue'
import CompanyRequest from '@/views/Company/LoanRequest/CompanyRequest.vue'
import LoanRequests from '@/views/User/LoanRequest/LoanRequests.vue'
import LoanRequest from '@/views/User/LoanRequest/LoanRequest.vue'
import CompanyLoans from '@/views/Company/Loan/CompanyLoans.vue'
import CompanyLoan from '@/views/Company/Loan/CompanyLoan.vue'
import Loans from '@/views/User/Loan/Loans.vue'
import Loan from '@/views/User/Loan/Loan.vue'
import LoanOffers from '@/views/User/LoanOffer/LoanOffers.vue';
import LoanOffer from '@/views/User/LoanOffer/LoanOffer.vue';


const routes = [
  {
    path: '/', name: 'Index', component: Index
  },
  {
    path: '/home', name: 'Home', component: Home, meta: { requiresAuth: true }
  },
  {
    path: '/login', name: 'Login', component: Login
  },
  {
    path: '/register', name: 'Register', component: Register
  },
  {
    path: '/companies', name: 'Companies', component: Companies, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id', name: 'Company', component: Company, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id/loan-requests/create', name: 'Create request', component: CreateRequest, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id/members', name: 'Company members', component: CompanyMembers, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id/loan-requests', name: 'Company requests', component: CompanyRequests, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id/loan-request/:loanRequestId', name: 'Company request', component: CompanyRequest, meta: { requiresAuth: true }
  },
  {
    path: '/loan-requests', name: 'Loan requests', component: LoanRequests, meta: { requiresAuth: true }
  },
  {
    path: '/loan-request/:id', name: 'Loan request', component: LoanRequest, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id/loans', name: 'Company loans', component: CompanyLoans, meta: { requiresAuth: true }
  },
  {
    path: '/company/:id/loan/:loanId', name: 'Company loan', component: CompanyLoan, meta: { requiresAuth: true }
  },
  {
    path: '/loans', name: 'Loans', component: Loans, meta: { requiresAuth: true }
  },
  {
    path: '/loan/:id', name: 'Loan', component: Loan, meta: { requiresAuth: true }
  },
  {
    path: '/loan-offers', name: 'Loan offers', component: LoanOffers, meta: { requiresAuth: true }
  },
  {
    path: '/loan-offers/:id', name: 'Loan offer', component: LoanOffer, meta: { requiresAuth: true }
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login');
  } else {
    next();
  }
});

export default router

import type { ILoan } from "./ILoan";
import type { IUserCompany } from "./IUserCompany";
import type { ILoanOffer } from "./ILoanOffer";


export interface IHomePageData {
    userCompanies: IUserCompany[];
    userLoans: ILoan[];
    userLoanOffers: ILoanOffer[];
}
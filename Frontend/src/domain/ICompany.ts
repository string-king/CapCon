import type { ILoan } from "./ILoan";
import type { ILoanOffer } from "./ILoanOffer";
import type { ILoanRequest } from "./ILoanRequest";
import type { IUserCompany } from "./IUserCompany";

export default interface ICompany {
    id: string;
    companyName: string;
    userCompanies: IUserCompany[];
    loanRequests: ILoanRequest[];
    loanOffers: ILoanOffer[];
    loans: ILoan[];

}
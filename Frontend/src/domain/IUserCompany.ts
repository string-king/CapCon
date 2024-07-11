import type ICompany from "./ICompany";
import type { IUser } from "./IUser";
import type { ECompanyRole } from "./enums/ECompanyRole";

export interface IUserCompany {
    id: string;
    appUserId: string;
    appUser: IUser;
    companyId: string;
    company: ICompany;
    role: ECompanyRole;
}

export interface IUserCompanyForCompany {
    id: string;
    appUserId: string;
    appUser: IUser;
    companyId?: string;
    companyRole: ECompanyRole;
}